using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectD.Models;
using Xunit;

namespace ProjectD.Tests
{
    public class ProductModelTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void Product_WithValidData_IsValid()
        {
            var product = new Product
            {
                ProductName = "Test Product",
                SKU = "TP001",
                WeightKg = 5,
                Material = "Plastic",
                BatchNumber = 100,
                Price = 19.99,
                Category = "Electronics",
                ExpirationDate = DateTime.UtcNow.AddYears(1),
                IsDeleted = false
            };

            var results = ValidateModel(product);

            Assert.Empty(results);
        }

        [Fact]
        public void Product_WithoutProductName_IsInvalid()
        {
            var product = new Product
            {
                ProductName = "",
                Price = 9.99
            };

            var results = ValidateModel(product);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.ProductName)));
        }

        [Fact]
        public void Product_WithNegativePrice_IsInvalid()
        {
            var product = new Product
            {
                ProductName = "Invalid Product",
                Price = -5.00
            };

            var results = ValidateModel(product);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Price)));
        }

        [Fact]
        public void Product_WithZeroPrice_IsValid()
        {
            var product = new Product
            {
                ProductName = "Free Sample",
                Price = 0.0
            };

            var results = ValidateModel(product);

            Assert.Empty(results);
        }

        [Fact]
        public void Product_WithoutOptionalFields_IsStillValid()
        {
            var product = new Product
            {
                ProductName = "Simple Product",
                Price = 5.0
            };

            var results = ValidateModel(product);

            Assert.Empty(results);
        }

        [Fact]
        public void Product_ExpiredDate_InThePast_IsAllowed()
        {
            var product = new Product
            {
                ProductName = "Expired Product",
                Price = 1.0,
                ExpirationDate = DateTime.UtcNow.AddDays(-10)
            };

            var results = ValidateModel(product);

            Assert.Empty(results);
        }
    }
}
