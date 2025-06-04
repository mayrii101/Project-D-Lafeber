using System;
using System.Threading.Tasks;
using ProjectD.Models;
using ProjectD.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AzureSqlConnectionDemo.Tests.Services
{
    public class XmlToSqlImporterTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly XmlToSqlImporter _importer;
        private readonly Mock<ILogger<XmlToSqlImporter>> _loggerMock;

        public XmlToSqlImporterTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            _loggerMock = new Mock<ILogger<XmlToSqlImporter>>();

            _importer = new XmlToSqlImporter(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task ImportFromTwoXmlStringsAsync_ShouldImportCustomersAndOrders()
        {
            var xml1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Root>
  <Customer>
    <Id>1</Id>
    <BedrijfsNaam>Test Company</BedrijfsNaam>
    <ContactPersoon>John Doe</ContactPersoon>
    <Email>john@example.com</Email>
    <TelefoonNummer>123456789</TelefoonNummer>
    <Adres>123 Test Street</Adres>
    <IsDeleted>false</IsDeleted>
  </Customer>
</Root>";

            var xml2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Root>
  <Order>
    <Id>1</Id>
    <CustomerId>1</CustomerId>
    <Status>Pending</Status>
    <OrderDate>2025-05-21T00:00:00</OrderDate>
    <DeliveryAddress>123 Delivery St</DeliveryAddress>
    <ExpectedDeliveryDate>2025-05-25T00:00:00</ExpectedDeliveryDate>
    <ActualDeliveryDate></ActualDeliveryDate>
    <IsDeleted>false</IsDeleted>
  </Order>
</Root>";

            // Act
            await _importer.ImportFromTwoXmlStringsAsync(xml1, xml2);

            // Assert Customer was inserted
            var customer = await _context.Customers.FindAsync(1);
            Assert.NotNull(customer);
            Assert.Equal("Test Company", customer.BedrijfsNaam);

            // Assert Order was inserted
            var order = await _context.Orders.FindAsync(1);
            Assert.NotNull(order);
            Assert.Equal(1, order.CustomerId);
            Assert.Equal(OrderStatus.Pending, order.Status);

            // Optionally, verify logger was called for success
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Data imported successfully")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}