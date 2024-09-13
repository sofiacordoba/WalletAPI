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
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Xunit.Abstractions;

namespace Kata.Wallet.Tests
{
    public class WalletServiceTests
    {
        private readonly Mock<IWalletRepository> _walletRepositoryMock; 
        private readonly Mock<IMapper> _mapperMock;
        private readonly WalletService _walletService;
        public WalletServiceTests() 
        {
            _walletRepositoryMock = new Mock<IWalletRepository>();
            _mapperMock = new Mock<IMapper>();

            _walletService = new WalletService(_walletRepositoryMock.Object, _mapperMock.Object);
        }

        // Test to verify that a wallet is successfully created when valid data is provided
        [Fact]
        public async Task CreateWallet_ReturnsWalletDto_WhenDataIsValid()
        {
            // Arrange: mock wallet DTO and corresponding domain wallet
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            var wallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = walletDto.UserName,
                Currency = walletDto.Currency,
                UserDocument = walletDto.UserDocument
            };

            // Simulate repository returning null (indicating no duplicate accounts)
            _walletRepositoryMock.Setup(repo => repo.GetByDocumentAndCurrencyAsync(walletDto.UserDocument, walletDto.Currency))
                     .ReturnsAsync((Kata.Wallet.Domain.Wallet?)null);

            // Simulate the repository saving the wallet correctly
            _walletRepositoryMock.Setup(repo => repo.CreateAsync(It.IsAny<Kata.Wallet.Domain.Wallet>()))
                                 .ReturnsAsync(wallet);

            // Simulate AutoMapper mapping between DTO and domain model
            _mapperMock.Setup(mapper => mapper.Map<Kata.Wallet.Domain.Wallet>(walletDto)).Returns(wallet);
            _mapperMock.Setup(mapper => mapper.Map<WalletDto>(wallet)).Returns(walletDto);

            // Act
            var result = await _walletService.CreateWalletAsync(walletDto);

            // Assert: verify that the result is not null and has the correct data
            Assert.NotNull(result);  
            Assert.Equal(walletDto.UserName, result.UserName);
            Assert.Equal(walletDto.Currency, result.Currency);
            Assert.Equal(walletDto.UserDocument, result.UserDocument);
        }

        // Test to verify that an exception is thrown when a wallet with the same currency and document exists
        [Fact]
        public async Task CreateWallet_ThrowsInvalidOperationException_WhenAccountExists()
        {
            // Arrange: mock wallet DTO and an existing wallet
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            var existingWallet = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            // Simulate repository returning an existing wallet (indicating a duplicate account)
            _walletRepositoryMock.Setup(repo => repo.GetByDocumentAndCurrencyAsync(walletDto.UserDocument, walletDto.Currency))
                                 .ReturnsAsync(existingWallet);

            // Act & Assert: verify that the correct exception is thrown
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _walletService.CreateWalletAsync(walletDto));

            // Assert the exception message is as expected
            Assert.Equal("The user already has an account with this currency.", exception.Message);
        }

        // Test to verify filtering by currency and user document in the GetAllAsync method
        [Fact]
        public async Task GetAllAsync_FiltersCorrectlyByCurrencyAndDocument()
        {
            // Arrange: mock two wallets and set up repository to return one of them based on the filter
            var wallet1 = new Kata.Wallet.Domain.Wallet
            {
                Id = 1,
                UserName = "TestUser1",
                Currency = "USD",
                UserDocument = "12345678"
            };

            _mapperMock.Setup(m => m.Map<WalletDto>(It.IsAny<Kata.Wallet.Domain.Wallet>()))
                .Returns((Kata.Wallet.Domain.Wallet w) => new WalletDto
                {
                    Id = w.Id,
                    UserName = w.UserName,
                    Currency = w.Currency,
                    UserDocument = w.UserDocument
                });

            // Simulate repository returning filtered wallet
            _walletRepositoryMock.Setup(repo => repo.GetAllAsync(It.IsAny<string?>(), It.IsAny<string?>()))
             .ReturnsAsync(new List<Kata.Wallet.Domain.Wallet> { wallet1 });

            // Act
            var result = await _walletService.GetAllAsync("USD", "12345678");

            // Assert: verify the result is not null or empty and contains the correct wallet
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            var firstWallet = result.FirstOrDefault();
            Assert.NotNull(firstWallet);
            Assert.Equal(wallet1.UserDocument, firstWallet.UserDocument);
            Assert.Equal(wallet1.Currency, firstWallet.Currency);
        }
    }
}