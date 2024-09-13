using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kata.Wallet.Persistence.Repositories
{
    // Repository class to manage wallet-related database operations
    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;

        public WalletRepository(DataContext context)
        {
            _context = context;
        }

        // Method to create a new wallet and save it in the database
        public async Task<Kata.Wallet.Domain.Wallet> CreateAsync(Kata.Wallet.Domain.Wallet wallet)
        {
            // Adds the new wallet to the context and saves changes in the database
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        // Method to retrieve a wallet by its ID
        public async Task<Kata.Wallet.Domain.Wallet?> GetByIdAsync(int id)
        {
            // Finds the wallet by its primary key (ID)
            return await _context.Wallets.FindAsync(id);
        }

        // Method to retrieve a wallet by user document and currency
        public async Task<Kata.Wallet.Domain.Wallet?> GetByDocumentAndCurrencyAsync(string userDocument, string currency)
        {
            // Looks for the wallet that matches the user document and currency
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserDocument == userDocument && w.Currency == currency);
        }

        // Method to retrieve all wallets with optional filters for currency and user document
        public async Task<IEnumerable<Kata.Wallet.Domain.Wallet>> GetAllAsync(string? currency, string? userDocument)
        {
            var query = _context.Wallets.AsQueryable();

            // Applies the currency and user document filter if provided
            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(w => w.Currency == currency);
            }

            if (!string.IsNullOrEmpty(userDocument))
            {
                query = query.Where(w => w.UserDocument == userDocument);
            }

            // Executes the query and returns the result
            return await query.ToListAsync();
            //Could add pagination!
        }
    }
}