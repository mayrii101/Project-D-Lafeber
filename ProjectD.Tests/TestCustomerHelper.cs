using AzureSqlConnectionDemo.Models;

namespace AzureSqlConnectionDemo.Test
{
    public static class TestCustomerHelper
    {
        public static Customer TestCustomer1 => new Customer
        {
            BedrijfsNaam = "Testbedrijf 1",
            ContactPersoon = "Jan Test",
            Email = "jan@test.com",
            TelefoonNummer = "0612345678",
            Adres = "Straat 1, Stad"
        };

        public static Customer TestCustomer2 => new Customer
        {
            BedrijfsNaam = "Testbedrijf 2",
            ContactPersoon = "Piet Update",
            Email = "piet@update.com",
            TelefoonNummer = "0687654321",
            Adres = "Straat 2, Andere Stad"
        };

        public static Customer CreateRandomCustomer()
        {
            return new Customer
            {
                BedrijfsNaam = Guid.NewGuid().ToString(),
                ContactPersoon = Guid.NewGuid().ToString(),
                Email = $"{Guid.NewGuid()}@test.com",
                TelefoonNummer = $"06{new Random().Next(10000000, 99999999)}",
                Adres = $"Straat {new Random().Next(1, 100)}, Stad"
            };
        }
    }
}
