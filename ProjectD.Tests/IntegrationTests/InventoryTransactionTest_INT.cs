/*
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ProjectD.Models;
using Xunit;

namespace ProjectD.IntegrationTests
{
    public class InventoryTransactionIntegrationTests : IDisposable
    {
        private readonly HttpClient _client;

        public InventoryTransactionIntegrationTests()
        {
            // Point this to your running API URL
            _client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };
        }

        [Fact]
        public async Task GetAllInventoryTransactions_ReturnsList()
        {
            var response = await _client.GetAsync("/api/InventoryTransaction");
            response.EnsureSuccessStatusCode();

            var transactions = await response.Content.ReadFromJsonAsync<List<InventoryTransaction>>();
            Assert.NotNull(transactions);
        }

        [Fact]
        public async Task CreateUpdateAndDeleteInventoryTransaction_WorksCorrectly()
        {
            // Create a new transaction
            var newTransaction = new InventoryTransaction
            {
                ProductId = 1,
                Quantity = 10,
                Type = InventoryTransactionType.Inbound, // adjust enum accordingly
                Timestamp = DateTime.UtcNow,
                EmployeeId = 1,
                SourceOrDestination = "Supplier",
                IsDeleted = false
            };

            var createResponse = await _client.PostAsJsonAsync("/api/InventoryTransaction", newTransaction);
            createResponse.EnsureSuccessStatusCode();

            var createdTransaction = await createResponse.Content.ReadFromJsonAsync<InventoryTransaction>();
            Assert.NotNull(createdTransaction);
            Assert.Equal(newTransaction.ProductId, createdTransaction.ProductId);

            // Update the transaction
            createdTransaction.Quantity = 20;
            var updateResponse = await _client.PutAsJsonAsync($"/api/InventoryTransaction/{createdTransaction.Id}", createdTransaction);
            updateResponse.EnsureSuccessStatusCode();

            var updatedTransaction = await updateResponse.Content.ReadFromJsonAsync<InventoryTransaction>();
            Assert.Equal(20, updatedTransaction.Quantity);

            // Soft delete the transaction
            var deleteResponse = await _client.DeleteAsync($"/api/InventoryTransaction/{createdTransaction.Id}");
            Assert.True(deleteResponse.IsSuccessStatusCode);

            // Verify deletion by trying to get the deleted transaction
            var getDeletedResponse = await _client.GetAsync($"/api/InventoryTransaction/{createdTransaction.Id}");
            Assert.Equal(System.Net.HttpStatusCode.NotFound, getDeletedResponse.StatusCode);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
*/ 