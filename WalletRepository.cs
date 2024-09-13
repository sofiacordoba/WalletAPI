using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kata.Wallet.Persistence.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext _context;

        public WalletRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet?> GetByIdAsync(int id)
        {
            return await _context.Wallets.FindAsync(id);
        }

        public async Task<Wallet?> GetByDocumentAndCurrencyAsync(string userDocument, string currency)
        {
            return await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserDocument == userDocument && w.Currency == currency);
        }

        public async Task<IEnumerable<Wallet>> GetAllAsync(string? currency, string? userDocument)
        {
            var query = _context.Wallets.AsQueryable();

            if (!string.IsNullOrEmpty(currency))
            {
                query = query.Where(w => w.Currency == currency);
            }

            if (!string.IsNullOrEmpty(userDocument))
            {
                query = query.Where(w => w.UserDocument == userDocument);
            }

            return await query.ToListAsync();
        }
    }
}
