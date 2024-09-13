using Kata.Wallet.Domain;

namespace Kata.Wallet.Persistence.Repositories
{
    // Interface for managing Wallet data in the persistence layer
    public interface IWalletRepository
    {
        // Method to create a new Wallet
        Task<Kata.Wallet.Domain.Wallet> CreateAsync(Kata.Wallet.Domain.Wallet wallet);

        // Method to retrieve a Wallet by its ID
        Task<Kata.Wallet.Domain.Wallet?> GetByIdAsync(int id);

        // Method to retrieve a Wallet by its User Document and Currency
        Task<Kata.Wallet.Domain.Wallet?> GetByDocumentAndCurrencyAsync(string userDocument, string currency);

        // Method to get all Wallets with optional filters for currency and user document
        Task<IEnumerable<Kata.Wallet.Domain.Wallet>> GetAllAsync(string? currency, string? userDocument);
    }
}
