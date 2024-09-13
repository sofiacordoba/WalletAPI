using Kata.Wallet.Dtos;
using Kata.Wallet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(ITransactionService transactionService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        // Endpoint to create a transaction (transfer between wallets)
        [HttpPost("transfer")]
        public async Task<ActionResult<TransactionDto>> CreateTransfer([FromBody] TransferRequestDto transferRequest)
        {
            try
            {
                var transaction = await _transactionService.CreateTransferAsync(transferRequest);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                // Log and return 404 if the source or target wallet is not found
                _logger.LogWarning(ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex.Message);
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log and return 500 for any other unexpected errors
                _logger.LogError($"Error creating the transfer: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the transfer.");
            }
        }

        // Endpoint to get transactions for a specific wallet by wallet ID
        [HttpGet("{walletId}/transactions")]
        public async Task<ActionResult<List<TransactionDto>>> GetTransactions(int walletId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByWalletIdAsync(walletId);
                if (transactions == null || transactions.Count == 0)
                {
                    return NotFound($"No transactions found for the wallet with ID: {walletId}.");
                }
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions.");
                return StatusCode(500, "An error occurred while retrieving transactions.");
            }
        }
    }
}
