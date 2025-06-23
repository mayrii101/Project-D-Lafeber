using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ProjectD.Models;
using Xunit;

namespace ProjectD.Tests
{
    public class CustomerModelTests
    {
        private List<ValidationResult> ValidateModel(object model)
        {
            var context = new ValidationContext(model, null, null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, context, results, true);
            return results;
        }

        [Fact]
        public void Customer_WithValidData_IsValid()
        {
            var customer = new Customer
            {
                BedrijfsNaam = "Acme B.V.",
                ContactPersoon = "Jan Jansen",
                Email = "jan@acme.nl",
                TelefoonNummer = "0612345678",
                Adres = "Teststraat 1",
                IsDeleted = false
            };

            var results = ValidateModel(customer);

            Assert.Empty(results);
        }

        [Fact]
        public void Customer_WithoutBedrijfsNaam_IsInvalid()
        {
            var customer = new Customer
            {
                ContactPersoon = "Piet Pietersen",
                Email = "piet@bedrijf.nl"
            };

            var results = ValidateModel(customer);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Customer.BedrijfsNaam)));
        }

        [Fact]
        public void Customer_WithoutContactPersoon_IsInvalid()
        {
            var customer = new Customer
            {
                BedrijfsNaam = "Test B.V.",
                Email = "info@test.nl"
            };

            var results = ValidateModel(customer);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Customer.ContactPersoon)));
        }

        [Fact]
        public void Customer_WithInvalidEmail_IsInvalid()
        {
            var customer = new Customer
            {
                BedrijfsNaam = "Fake B.V.",
                ContactPersoon = "Fake Persoon",
                Email = "not-an-email"
            };

            var results = ValidateModel(customer);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Customer.Email)));
        }

        [Fact]
        public void Customer_WithoutEmail_IsInvalid()
        {
            var customer = new Customer
            {
                BedrijfsNaam = "NoEmail B.V.",
                ContactPersoon = "Emma Zondermail",
                Email = "" // still invalid
            };

            var results = ValidateModel(customer);

            Assert.Contains(results, r => r.MemberNames.Contains(nameof(Customer.Email)));
        }


        [Fact]
        public void Customer_WithoutOptionalFields_IsValid()
        {
            var customer = new Customer
            {
                BedrijfsNaam = "Minimal B.V.",
                ContactPersoon = "Minimalist",
                Email = "minimal@b.v"
                // TelefoonNummer and Adres are optional
            };

            var results = ValidateModel(customer);

            Assert.Empty(results);
        }
    }
}
