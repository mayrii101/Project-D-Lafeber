using Microsoft.EntityFrameworkCore;
using ProjectD.Models;

namespace ProjectD.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(int id, Product product);
        Task<bool> SoftDeleteProductAsync(int id);
    }


    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                                 .Where(p => !p.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _context.Products
                                 .Where(p => p.Id == id && !p.IsDeleted)
                                 .FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null) return null;

            existingProduct.ProductName = product.ProductName;
            existingProduct.WeightKg = product.WeightKg;
            existingProduct.Material = product.Material;
            existingProduct.BatchNumber = product.BatchNumber;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;

            await _context.SaveChangesAsync();
            return existingProduct;
        }


        public async Task<bool> SoftDeleteProductAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted) return false;

            product.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}