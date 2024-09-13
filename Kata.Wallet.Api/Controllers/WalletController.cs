using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Domain;
using Kata.Wallet.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Kata.Wallet.Services.Interfaces;

namespace Kata.Wallet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;
    private readonly ILogger<WalletController> _logger;

    public WalletController(IWalletService walletService, ILogger<WalletController> logger)
    {
        _walletService = walletService;
        _logger = logger;
    }

    // Endpoint to retrieve all wallets with optional filters (currency and user document)
    [HttpGet]
    public async Task<ActionResult<List<WalletDto>>> GetAll([FromQuery] string? currency, [FromQuery] string? userDocument)
    {
        try
        {
            // Call service to get filtered wallets based on currency and user document
            var walletDtos = await _walletService.GetAllAsync(currency, userDocument);
            return Ok(walletDtos);
        }
        catch (ArgumentException ex)
        {
            // Handle invalid argument (e.g., invalid currency format)
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            // Log and return 500 for any unexpected error during wallet retrieval
            _logger.LogError($"Error retrieving wallets: {ex.Message}");
            return StatusCode(500, "An error occurred while retrieving the wallets: " + ex.Message);
        }
    }

    // Endpoint to create a new wallet
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] WalletDto wallet)
    {
        // Model validation
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid wallet data.");
            return BadRequest(ModelState);
        }

        try
        {
            // Call the service to create a new wallet
            var createdWalletDto = await _walletService.CreateWalletAsync(wallet);
            if (createdWalletDto == null)
            {
                _logger.LogError("Error creating wallet: Service returned null.");
                return StatusCode(500, "Error creating wallet.");
            }

            _logger.LogInformation($"Wallet successfully created. ID: {createdWalletDto.Id}, User: {wallet.UserName}, Document: {wallet.UserDocument}.");
            return Ok(createdWalletDto);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Account already exists"))
        {
            // Handle case where a wallet already exists with the same currency and user document
            _logger.LogWarning(ex.Message);
            return BadRequest(new { Message = ex.Message });
        }
        catch (DbUpdateException dbEx)
        {
            // Log and handle database update error
            _logger.LogError(dbEx, "Database error occurred while saving data.");
            return StatusCode(500, "An error occurred while saving the data to the database.");
        }
        catch (Exception ex)
        {
            // Log and handle any other unexpected error
            _logger.LogError(ex, $"Error creating wallet for user {wallet.UserName}.");
            return StatusCode(500, "An error occurred while creating the wallet.");
        }
    }
}
