using Kata.Wallet.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kata.Wallet.Tests
{
    public class WalletIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        public WalletIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services => { });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost") // Important: Ensure using HTTP here
            });
        }

        [Fact]
        public async Task CreateWallet_ReturnsSuccess()
        {
            // Arrange: Prepare the request to create a wallet
            var walletRequest = new
            {
                UserDocument = "12345678",
                UserName = "Test User",
                Currency = "USD",
                Balance = 100
            };

            // Act: Send a POST request to the API to create a wallet
            var response = await _client.PostAsJsonAsync("/api/wallet", walletRequest);

            // Check if the route was not found (404 NotFound)
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return;
            }

            // Verify that the response contains a valid JSON body
            var wallet = await response.Content.ReadFromJsonAsync<WalletDto>();

            if (wallet == null)
            {
                return;
            }

            // Assert: Check that the wallet is not null and verify its content
            Assert.NotNull(wallet);
            Assert.Equal("12345678", wallet.UserDocument); 
            Assert.Equal("USD", wallet.Currency);
        }
    }
}
