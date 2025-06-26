using Microsoft.AspNetCore.Mvc;
using ProjectD.Models;
using ProjectD.Services;

namespace AzureSqlConnectionDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> UpdateProduct(int id, Product product)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> SoftDeleteProduct(int id)
        {
            var deleted = await _productService.SoftDeleteProductAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}