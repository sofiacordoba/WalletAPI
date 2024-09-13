using Kata.Wallet.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata.Wallet.Services.Interfaces
{
    // Interface defining the contract for wallet-related services
    public interface IWalletService
    {
        // Method to retrieve a list of wallets with optional filters for currency and user document
        Task<List<WalletDto>> GetAllAsync(string? currency, string? userDocument);

        // Method to create a new wallet based on the provided WalletDto
        Task<WalletDto> CreateWalletAsync(WalletDto walletDto);
    }
}
