using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Domain;
using Kata.Wallet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Kata.Wallet.Persistence.Repositories;

namespace Kata.Wallet.Services
{
    // Implementation of the IWalletService interface
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        public WalletService(IWalletRepository walletRepository, IMapper mapper)
        {
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        // Method to create a new wallet, ensuring no duplicate accounts
        public async Task<WalletDto> CreateWalletAsync(WalletDto walletDto)
        {
            // Normalize input data
            walletDto.Currency = walletDto.Currency?.Trim().Replace(" ", "").ToUpper() ?? string.Empty;
            walletDto.UserDocument = walletDto.UserDocument?.Trim() ?? string.Empty;

            // Check if a wallet with the same user document and currency already exists
            var existingWallet = await _walletRepository.GetByDocumentAndCurrencyAsync(walletDto.UserDocument, walletDto.Currency);

            if (existingWallet != null)
            {
                throw new InvalidOperationException("The user already has an account with this currency.");
            }

            // Map the WalletDto to a Wallet entity and create the wallet
            var newWallet = _mapper.Map<Kata.Wallet.Domain.Wallet>(walletDto);
            await _walletRepository.CreateAsync(newWallet);

            // Return the newly created wallet as a DTO
            return _mapper.Map<WalletDto>(newWallet);
        }

        // Method to retrieve a list of wallets, optionally filtered by currency and user document
        public async Task<List<WalletDto>> GetAllAsync(string? currency, string? userDocument)
        {
            // Normalize currency and user document if provided
            if (!string.IsNullOrEmpty(currency))
            {
                currency = currency.Trim().ToUpper();
            }

            if (!string.IsNullOrEmpty(userDocument))
            {
                userDocument = userDocument.Trim();
            }

            // Fetch wallets from the repository with optional filters
            var wallets = await _walletRepository.GetAllAsync(currency, userDocument);

            // Map the wallet entities to DTOs and return the result
            return wallets.Select(wallet => _mapper.Map<WalletDto>(wallet)).ToList();
        }
    }
}
