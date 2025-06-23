using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectD.Models;
using ProjectD.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class CustomerServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _service = new CustomerService(_context);

            _context.Customers.AddRange(
                new Customer { Id = 1, BedrijfsNaam = "Test A", IsDeleted = false },
                new Customer { Id = 2, BedrijfsNaam = "Test B", IsDeleted = true }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllCustomersAsync_ShouldReturnOnlyNonDeleted()
        {
            var result = await _service.GetAllCustomersAsync();

            Assert.Single(result);
            Assert.Equal("Test A", result.First().BedrijfsNaam);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnCorrectCustomer()
        {
            var result = await _service.GetCustomerByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Test A", result.BedrijfsNaam);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_ShouldReturnNullIfDeleted()
        {
            var result = await _service.GetCustomerByIdAsync(2);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldAddCustomer()
        {
            var newCustomer = new Customer
            {
                BedrijfsNaam = "New Co",
                ContactPersoon = "Jan",
                Email = "jan@example.com",
                TelefoonNummer = "1234567890",
                Adres = "Teststraat 1"
            };

            var result = await _service.CreateCustomerAsync(newCustomer);
            var customers = await _context.Customers.ToListAsync();

            Assert.Equal(3, customers.Count);
            Assert.Equal("New Co", result.BedrijfsNaam);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldUpdateCustomer()
        {
            var update = new Customer
            {
                BedrijfsNaam = "Updated Co",
                ContactPersoon = "Pieter",
                Email = "pieter@example.com",
                TelefoonNummer = "0000000000",
                Adres = "Nieuwstraat 99"
            };

            var result = await _service.UpdateCustomerAsync(1, update);

            Assert.NotNull(result);
            Assert.Equal("Updated Co", result.BedrijfsNaam);
        }

        [Fact]
        public async Task UpdateCustomerAsync_ShouldReturnNullIfNotFound()
        {
            var update = new Customer { BedrijfsNaam = "Non-existent" };
            var result = await _service.UpdateCustomerAsync(999, update);

            Assert.Null(result);
        }

        [Fact]
        public async Task SoftDeleteCustomerAsync_ShouldSetIsDeleted()
        {
            var result = await _service.SoftDeleteCustomerAsync(1);
            var customer = await _context.Customers.FindAsync(1);

            Assert.True(result);
            Assert.True(customer.IsDeleted);
        }

        [Fact]
        public async Task SoftDeleteCustomerAsync_ShouldReturnFalseIfAlreadyDeleted()
        {
            var result = await _service.SoftDeleteCustomerAsync(2);
            Assert.False(result);
        }

        [Fact]
        public async Task SoftDeleteCustomerAsync_ShouldReturnFalseIfNotFound()
        {
            var result = await _service.SoftDeleteCustomerAsync(999);
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
