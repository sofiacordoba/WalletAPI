using Kata.Wallet.Domain;

namespace Kata.Wallet.Persistence.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet?> GetByIdAsync(int id);
        Task<Wallet?> GetByDocumentAndCurrencyAsync(string userDocument, string currency);
        Task<IEnumerable<Wallet>> GetAllAsync(string? currency, string? userDocument);
    }
}