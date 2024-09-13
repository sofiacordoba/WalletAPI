using Kata.Wallet.Database;
using Kata.Wallet.Domain;
using Microsoft.EntityFrameworkCore;

namespace Kata.Wallet.Persistence.Repositories
{
    // Repository class to manage transaction-related database operations
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DataContext _context;

        public TransactionRepository(DataContext context)
        {
            _context = context;
        }

        // Method to create a new transaction and save it in the database
        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        // Method to retrieve a transaction by its ID
        public async Task<Transaction?> GetByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        // Method to get all transactions related to a specific wallet ID
        public async Task<IEnumerable<Transaction>> GetAllByWalletIdAsync(int walletId)
        {
            // Fetches all transactions where the wallet is either the sender or the receiver
            return await _context.Transactions
                .Where(t => t.WalletIncoming.Id == walletId || t.WalletOutgoing.Id == walletId)
                .ToListAsync();
        }
    }
}
