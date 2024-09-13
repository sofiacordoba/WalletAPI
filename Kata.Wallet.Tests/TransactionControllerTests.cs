using Moq;
using Xunit;
using Kata.Wallet.Api.Controllers;
using Kata.Wallet.Services.Interfaces;
using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kata.Wallet.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<ILogger<TransactionController>> _loggerMock;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _loggerMock = new Mock<ILogger<TransactionController>>();
            _controller = new TransactionController(_transactionServiceMock.Object, _loggerMock.Object);
        }

        // Test to verify that a valid transfer returns an OK result with correct data
        [Fact]
        public async Task CreateTransfer_ReturnsOk_WhenTransferIsValid()
        {
            // Arrange: mock a valid transfer request and the expected result
            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 100
            };

            var transactionDto = new TransactionDto
            {
                Id = 1,
                Amount = 100,
                Date = System.DateTime.Now,
                Description = "Successful transfer"
            };

            // Set up the mock to return the transaction DTO
            _transactionServiceMock
                .Setup(service => service.CreateTransferAsync(transferRequest))
                .ReturnsAsync(transactionDto);

            // Act
            var result = await _controller.CreateTransfer(transferRequest);

            // Assert: verify that the result is an OK response with the expected transaction details
            var okResult = Assert.IsType<OkObjectResult>(result.Result);  
            var returnedTransaction = Assert.IsType<TransactionDto>(okResult.Value); 
            Assert.Equal(100, returnedTransaction.Amount);  
        }

        // Test to verify that a transfer with different currencies returns a BadRequest
        [Fact]
        public async Task CreateTransfer_ReturnsBadRequest_WhenCurrenciesAreDifferent()
        {
            // Arrange: mock a transfer request where source and target wallets have different currencies
            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 100
            };

            // Simulate an InvalidOperationException for different currencies
            _transactionServiceMock
            .Setup(service => service.CreateTransferAsync(transferRequest))
            .ThrowsAsync(new InvalidOperationException("Cannot transfer between wallets with different currencies"));

            // Act
            var result = await _controller.CreateTransfer(transferRequest);

            // Assert: verify BadRequest with the correct error message
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);

            
            Assert.NotNull(badRequestResult.Value);
            var responseValue = badRequestResult.Value.ToString();
            Assert.Contains("Cannot transfer between wallets with different currencies", responseValue);
        }

        // Test to verify that a transfer to a non-existent account returns NotFound
        [Fact]
        public async Task CreateTransfer_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange: mock a transfer request with a non-existent target wallet
            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 99,
                Amount = 100
            };

            // Simulate a KeyNotFoundException for a non-existent target wallet
            _transactionServiceMock
                .Setup(service => service.CreateTransferAsync(It.IsAny<TransferRequestDto>()))
                .ThrowsAsync(new KeyNotFoundException("Target account not found"));

            // Act
            var result = await _controller.CreateTransfer(transferRequest);

            // Assert: verify NotFound with the correct error message
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);

            // Imprimir el valor devuelto para depuración
            Console.WriteLine(notFoundResult.Value);

            // Assert: Verifica que el valor no sea null
            Assert.NotNull(notFoundResult.Value);

            // Convertir el valor devuelto a un diccionario y verificar la propiedad Message
            Assert.NotNull(notFoundResult.Value);
            Assert.Equal("Target account not found", notFoundResult.Value.ToString());
        }

        // Test to verify that an invalid transfer operation returns a BadRequest
        [Fact]
        public async Task CreateTransfer_ReturnsBadRequest_WhenServiceThrowsInvalidOperationException()
        {
            // Arrange: mock a transfer request that leads to an invalid operation (e.g., insufficient funds)
            var transferRequest = new TransferRequestDto
            {
                SourceWalletId = 1,
                TargetWalletId = 2,
                Amount = 100
            };

            _transactionServiceMock
                .Setup(service => service.CreateTransferAsync(It.IsAny<TransferRequestDto>()))
                .ThrowsAsync(new System.InvalidOperationException("Insufficient funds"));

            // Act
            var result = await _controller.CreateTransfer(transferRequest);

            // Assert: verify BadRequest with the correct error message
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);  
            Assert.Equal(400, badRequestResult.StatusCode);  
            Assert.NotNull(badRequestResult.Value);  
            var responseValue = badRequestResult.Value.ToString();
            Assert.Contains("Insufficient funds", responseValue);
        }

        // Test to verify that GetTransactions returns Ok when transactions are available
        [Fact]
        public async Task GetTransactions_ReturnsOk_WhenTransactionsExist()
        {
            // Arrange: mock a wallet ID and a list of transactions
            var walletId = 1;
            var transactionList = new List<TransactionDto>
            {
                new TransactionDto { Id = 1, Amount = 100, Date = System.DateTime.Now },
                new TransactionDto { Id = 2, Amount = 50, Date = System.DateTime.Now }
            };

            _transactionServiceMock
                .Setup(service => service.GetTransactionsByWalletIdAsync(walletId))
                .ReturnsAsync(transactionList);

            // Act
            var result = await _controller.GetTransactions(walletId);

            // Assert: verify that Ok is returned with the expected transactions
            Assert.NotNull(result);
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult); 
            Assert.Equal(200, okResult.StatusCode);
            var responseValue = okResult.Value as List<TransactionDto>;
            Assert.NotNull(responseValue); 
            Assert.Equal(2, responseValue.Count);
        }
    }
}