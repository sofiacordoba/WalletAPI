using Kata.Wallet.Domain;

namespace Kata.Wallet.Persistence.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetByWalletIdAsync(int walletId);
        Task<bool> ExistsWalletAsync(int walletId);
    }
}
