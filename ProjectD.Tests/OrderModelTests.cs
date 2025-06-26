using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ProjectD.Models;
using Xunit;

namespace ProjectD.Tests
{
    public class OrderModelTests
    {
        [Fact]
        public void Order_WithValidData_IsValid()
        {
            var order = new Order
            {
                CustomerId = 1,
                OrderDate = DateTime.UtcNow,
                ExpectedDeliveryDate = DateTime.UtcNow.AddDays(3),
                DeliveryAddress = "Teststraat 5",
                Status = OrderStatus.Pending,
                ProductLines = new List<OrderLine>
                {
                    new OrderLine
                    {
                        Product = new Product { Price = 10.0, WeightKg = 2 },
                        Quantity = 3
                    }
                }
            };

            var context = new ValidationContext(order);
            var results = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(order, context, results, true);

            Assert.True(isValid);
            Assert.Equal(6, order.TotalWeight);
        }

        [Fact]
        public void OrderLine_CalculatesLineTotalCorrectly()
        {
            var product = new Product
            {
                Price = 12.5,
                WeightKg = 1
            };

            var orderLine = new OrderLine
            {
                Product = product,
                Quantity = 4
            };

            Assert.Equal(50.0, orderLine.LineTotal);
        }

        [Fact]
        public void OrderLine_WithZeroQuantity_StillValid_ButTotalIsZero()
        {
            var orderLine = new OrderLine
            {
                Product = new Product { Price = 5.0, WeightKg = 2 },
                Quantity = 0
            };

            Assert.Equal(0, orderLine.LineTotal);
            Assert.Equal(0, orderLine.Product.WeightKg * orderLine.Quantity);
        }

        [Fact]
        public void Order_TotalWeight_CalculatesCorrectlyWithMultipleLines()
        {
            var order = new Order
            {
                ProductLines = new List<OrderLine>
                {
                    new OrderLine { Product = new Product { WeightKg = 5 }, Quantity = 2 },
                    new OrderLine { Product = new Product { WeightKg = 3 }, Quantity = 1 }
                }
            };

            Assert.Equal(13, order.TotalWeight);
        }
    }
}
