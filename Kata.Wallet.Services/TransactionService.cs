using AutoMapper;
using Kata.Wallet.Dtos;
using Kata.Wallet.Domain;
using Kata.Wallet.Database;
using Kata.Wallet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Kata.Wallet.Persistence.Repositories;

namespace Kata.Wallet.Services
{
    // Service responsible for handling transaction-related business logic
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository, IWalletRepository walletRepository, IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
            _mapper = mapper;
        }

        // Method to create a transfer between wallets
        public async Task<TransactionDto> CreateTransferAsync(TransferRequestDto transferRequest)
        {
            if (transferRequest.Amount <= 0)
            {
                throw new InvalidOperationException("The transfer amount must be greater than 0.");
            }

            // Retrieve the source and target wallets by their IDs
            var sourceWallet = await _walletRepository.GetByIdAsync(transferRequest.SourceWalletId);
            var targetWallet = await _walletRepository.GetByIdAsync(transferRequest.TargetWalletId);

            // Validate that both wallets exist
            if (sourceWallet == null || targetWallet == null)
            {
                throw new KeyNotFoundException("The source or target wallet does not exist.");
            }

            // Validate that both wallets use the same currency
            if (sourceWallet.Currency != targetWallet.Currency)
            {
                throw new InvalidOperationException("The wallets must have the same currency.");
            }

            // Validate that the source wallet has sufficient balance
            if (sourceWallet.Balance < transferRequest.Amount)
            {
                throw new InvalidOperationException("Insufficient balance in the source wallet.");
            }

            // Update the balances of both wallets
            sourceWallet.Balance -= transferRequest.Amount;
            targetWallet.Balance += transferRequest.Amount;

            // Create a transaction entity and set its details
            var transaction = new Transaction
            {
                Amount = transferRequest.Amount,
                Date = DateTime.Now,
                WalletOutgoing = sourceWallet,
                WalletIncoming = targetWallet,
                Description = $"Transfer from {sourceWallet.UserDocument} to {targetWallet.UserDocument}"
            };

            // Save the transaction in the repository
            await _transactionRepository.CreateAsync(transaction);

            // Map the Transaction entity to TransactionDto and return it
            return _mapper.Map<TransactionDto>(transaction);
        }

        // Method to retrieve all transactions for a specific wallet by its ID
        public async Task<List<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId)
        {
            var transactions = await _transactionRepository.GetAllByWalletIdAsync(walletId);
            return _mapper.Map<List<TransactionDto>>(transactions);
        }
    }
}
