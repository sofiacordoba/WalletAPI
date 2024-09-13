using Moq;
using Xunit;
using Kata.Wallet.Api.Controllers;
using Kata.Wallet.Services.Interfaces;
using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Kata.Wallet.Services;
using Kata.Wallet.Persistence.Repositories;
using AutoMapper;
using Kata.Wallet.Domain;

namespace Kata.Wallet.Tests
{
    public class TransactionServiceTests
    {
        // Mocks for repositories and AutoMapper
        private readonly Mock<IWalletRepository> _walletRepositoryMock;
        private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        // Instance of the service under test
        private readonly TransactionService _transactionService;
        public TransactionServiceTests()
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _transactionRepositoryMock = new Mock<ITransactionRepository>();
            _mapperMock = new Mock<IMapper>();

            _transactionService = new TransactionService(
                _transactionRepositoryMock.Object,
                _walletRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        // Test to verify that a successful transfer occurs when balance is sufficient and currencies match
        [Fact]
        public async Task CreateTransfer_SuccessfulTransfer_WhenSufficientBalanceAndSameCurrency()
        {
            // Arrange: mock valid wallets and transfer request
            var sourceWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = "SourceUser",
                Currency = "USD",
                UserDocument = "11111111",
                Balance = 200m
            };

            var targetWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 2,
                UserName = "TargetUser",
                Currency = "USD",
                UserDocument = "22222222",
                Balance = 100m
            };

            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 50m
            };

            // Set up mocks to return the wallets and simulate transaction creation
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(sourceWallet);
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(targetWallet);

            // Simulate transaction creation
            _transactionRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Transaction>()))
                .ReturnsAsync(new Transaction { Amount = 50m, Description = $"Transfer from {sourceWallet.UserDocument} to {targetWallet.UserDocument}" });

            // Configuración de AutoMapper para el mapeo de Transaction a TransactionDto
            _mapperMock.Setup(m => m.Map<TransactionDto>(It.IsAny<Transaction>()))
                .Returns(new TransactionDto { Amount = 50m, Description = $"Transfer from {sourceWallet.UserDocument} to {targetWallet.UserDocument}" });

            // Act: perform the transfer
            var result = await _transactionService.CreateTransferAsync(transferRequest);

            // Assert: verify the result and balances
            Assert.NotNull(result);
            Assert.Equal(50m, result.Amount);
            Assert.Equal($"Transfer from {sourceWallet.UserDocument} to {targetWallet.UserDocument}", result.Description);
            Assert.Equal(150m, sourceWallet.Balance);
            Assert.Equal(150m, targetWallet.Balance);
        }

        // Test to verify that an exception is thrown when there is insufficient balance
        [Fact]
        public async Task CreateTransfer_ThrowsInvalidOperationException_WhenInsufficientBalance()
        {
            // Arrange: mock a source wallet with insufficient balance
            var sourceWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = "SourceUser",
                Currency = "USD",
                UserDocument = "11111111",
                Balance = 20m
            };

            var targetWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 2,
                UserName = "TargetUser",
                Currency = "USD",
                UserDocument = "22222222",
                Balance = 100m
            };

            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 50m
            };

            // Set up mocks to return the wallets
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(sourceWallet);
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(targetWallet);

            // Act & Assert: verify that an exception is thrown due to insufficient balance
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.CreateTransferAsync(transferRequest));
            Assert.Equal("Insufficient balance in the source wallet.", exception.Message);
        }

        // Test to verify that an exception is thrown when currencies differ between wallets
        [Fact]
        public async Task CreateTransfer_ThrowsInvalidOperationException_WhenCurrenciesAreDifferent()
        {
            // Arrange: mock wallets with different currencies
            var sourceWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = "SourceUser",
                Currency = "USD",
                UserDocument = "11111111",
                Balance = 200m
            };

            var targetWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 2,
                UserName = "TargetUser",
                Currency = "EUR",
                UserDocument = "22222222",
                Balance = 100m
            };

            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 50m
            };

            // Set up mocks to return the wallets
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(sourceWallet);
            _walletRepositoryMock.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(targetWallet);

            // Act & Assert: verify that an exception is thrown due to different currencies
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.CreateTransferAsync(transferRequest));
            Assert.Equal("The wallets must have the same currency.", exception.Message);
        }
    }
}