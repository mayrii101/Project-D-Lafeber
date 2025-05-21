using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<Customer> CreateCustomerAsync(Customer customer);
        Task<Customer> UpdateCustomerAsync(int id, Customer customer);
        Task<bool> SoftDeleteCustomerAsync(int id);
    }
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;

        public CustomerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers
                                 .Where(c => !c.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers
                                 .Where(c => c.Id == id && !c.IsDeleted)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer> UpdateCustomerAsync(int id, Customer customer)
        {
            var existingCustomer = await _context.Customers.FindAsync(id);
            if (existingCustomer == null)
                return null;

            existingCustomer.BedrijfsNaam = customer.BedrijfsNaam;
            existingCustomer.ContactPersoon = customer.ContactPersoon;
            existingCustomer.Email = customer.Email;
            existingCustomer.TelefoonNummer = customer.TelefoonNummer;
            existingCustomer.Adres = customer.Adres;

            await _context.SaveChangesAsync();
            return existingCustomer;
        }

        public async Task<bool> SoftDeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null || customer.IsDeleted) return false;

            customer.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}