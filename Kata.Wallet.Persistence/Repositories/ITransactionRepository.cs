using Kata.Wallet.Domain;

namespace Kata.Wallet.Persistence.Repositories
{
    // Interface for managing Transaction data in the persistence layer
    public interface ITransactionRepository
    {
        // Method to create a new transaction
        Task<Transaction> CreateAsync(Transaction transaction);

        // Method to retrieve a transaction by its ID
        Task<Transaction?> GetByIdAsync(int id);

        // Method to get all transactions related to a specific wallet by wallet ID
        Task<IEnumerable<Transaction>> GetAllByWalletIdAsync(int walletId);
    }
}
