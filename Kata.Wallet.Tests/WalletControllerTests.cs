using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Kata.Wallet.API.Controllers;
using Kata.Wallet.Services.Interfaces;
using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kata.Wallet.Tests
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _walletServiceMock;
        private readonly Mock<ILogger<WalletController>> _loggerMock;
        private readonly WalletController _controller;

        public WalletControllerTests()
        {
            _walletServiceMock = new Mock<IWalletService>();
            _loggerMock = new Mock<ILogger<WalletController>>();
            _controller = new WalletController(_walletServiceMock.Object, _loggerMock.Object);
        }

        // Test to ensure GetAll returns 200 OK when wallets are available
        [Fact]
        public async Task GetAll_ReturnsOk_WhenWalletsAreAvailable()
        {
            // Arrange: mock wallet data and service response
            var wallets = new List<WalletDto>
            {
                new WalletDto { Id = 1, UserName = "User1", Currency = "USD" },
                new WalletDto { Id = 2, UserName = "User2", Currency = "EUR" }
            };
            _walletServiceMock.Setup(service => service.GetAllAsync(null, null))
                .ReturnsAsync(wallets);

            // Act
            var result = await _controller.GetAll(null, null);

            // Assert: check that the result is 200 OK with the correct data
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedWallets = Assert.IsType<List<WalletDto>>(okResult.Value);
            Assert.Equal(2, returnedWallets.Count);
        }

        // Test to ensure Create returns 200 OK when a valid wallet is created
        [Fact]
        public async Task Create_ReturnsOk_WhenWalletIsValid()
        {
            // Arrange: create a valid wallet DTO
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };
            _walletServiceMock.Setup(service => service.CreateWalletAsync(walletDto))
                .ReturnsAsync(walletDto);

            // Act
            var result = await _controller.Create(walletDto);

            // Assert: check that the result is 200 OK with the correct wallet data
            var okResult = Assert.IsType<OkObjectResult>(result);
            var createdWallet = Assert.IsType<WalletDto>(okResult.Value);
            Assert.Equal("TestUser", createdWallet.UserName);
        }

        // Test to ensure Create returns 400 Bad Request when the model is invalid
        [Fact]
        public async Task Create_ReturnsBadRequest_WhenWalletIsInvalid()
        {
            // Arrange: set up an invalid wallet DTO and add model state errors
            var invalidWalletDto = new WalletDto(); // Missing required fields
            _controller.ModelState.AddModelError("UserName", "UserName is required");
            _controller.ModelState.AddModelError("Currency", "Currency is required");

            // Act
            var result = await _controller.Create(invalidWalletDto);

            // Assert: check that the result is 400 Bad Request
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        // Test to ensure Create returns 400 Bad Request when a duplicate account exists
        [Fact]
        public async Task Create_ReturnsBadRequest_WhenAccountAlreadyExists()
        {
            // Arrange: create a wallet DTO with duplicate data
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            // Simulate that the service throws an exception for a duplicate account
            _walletServiceMock
                .Setup(service => service.CreateWalletAsync(walletDto))
                .ThrowsAsync(new InvalidOperationException("Account already exists with the same currency and document."));

            // Act
            var result = await _controller.Create(walletDto);

            // Assert: check that the result is 400 Bad Request and contains the correct error message
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            // Check that the response contains the correct "Message" property
            var responseValue = badRequestResult.Value;
            Assert.NotNull(responseValue);

            // Verify that the message property exists and has the correct value
            var messageProperty = responseValue.GetType().GetProperty("Message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(responseValue)?.ToString();
            Assert.Equal("Account already exists with the same currency and document.", message);
        }

        // Test to ensure Create returns 500 Internal Server Error when a general exception is thrown
        [Fact]
        public async Task Create_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange: create a wallet DTO and simulate an exception in the service
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            _walletServiceMock
                .Setup(service => service.CreateWalletAsync(walletDto))
                .ThrowsAsync(new System.Exception("Test Exception."));

            // Act
            var result = await _controller.Create(walletDto);

            // Assert: check that the result is 500 Internal Server Error
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("An error occurred while creating the wallet.", serverErrorResult.Value);
        }

        // Test to ensure Create returns 500 Internal Server Error when a DbUpdateException occurs
        [Fact]
        public async Task Create_ReturnsServerError_WhenDbUpdateExceptionThrown()
        {
            // Arrange: create a wallet DTO and simulate a database error in the service
            var walletDto = new WalletDto
            {
                UserName = "TestUser",
                Currency = "USD",
                UserDocument = "12345678"
            };

            _walletServiceMock
                .Setup(service => service.CreateWalletAsync(walletDto))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateException("Database error."));

            // Act
            var result = await _controller.Create(walletDto);

            // Assert: check that the result is 500 Internal Server Error with the correct message
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);
            Assert.Equal("An error occurred while saving the data to the database.", serverErrorResult.Value);
        }
    }
}