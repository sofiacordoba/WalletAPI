using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Services.Interfaces
{
    // Interface for transaction-related business logic
    public interface ITransactionService
    {
        // Method to create a transfer between wallets
        // Parameters: TransferRequestDto containing source wallet, target wallet, and amount
        // Returns: TransactionDto with details of the created transaction
        Task<TransactionDto> CreateTransferAsync(TransferRequestDto transferRequestDto);

        // Method to retrieve all transactions related to a specific wallet
        // Parameters: walletId (ID of the wallet)
        // Returns: A list of TransactionDto representing transactions for the wallet
        Task<List<TransactionDto>> GetTransactionsByWalletIdAsync(int walletId);
    }
}
