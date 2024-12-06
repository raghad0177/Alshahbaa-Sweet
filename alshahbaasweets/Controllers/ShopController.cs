using alshahbaasweets.DTO;
using alshahbaasweets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;



namespace alshahbaasweets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ShopController(MyDbContext context)
        {
            _context = context;
        }

        // POST: api/Shop
        [HttpPost("PostProducts")]
        public async Task<IActionResult> CreateShopEntry(int productId, string amount, decimal price)
        {
            try
            {
                var shopEntry = new Shop
                {
                    ProductId = productId,
                    Amount = amount,
                    Price = price
                };

                _context.Shops.Add(shopEntry);
                await _context.SaveChangesAsync();

                return Ok(shopEntry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }






        [HttpGet("ProductWithPricing/{productId}")]
        public async Task<IActionResult> GetProductWithPricing(int productId)
        {
            // Fetch the product details from the Products table
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Fetch all pricing options from the Shop table for the product
            var pricingOptions = await _context.Shops
                .Where(s => s.ProductId == productId)
                .Select(s => new
                {
                    Id = s.ShopId,
                    Amount = s.Amount,
                    Price = s.Price
                })
                .ToListAsync();

            // Construct the response object, excluding price and amount from Products table
            var productDetails = new
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,
                CategoryId = product.CategoryId,
                PricingOptions = pricingOptions // Only show pricing options from Shop table
            };

            return Ok(productDetails);
        }




        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Image = p.Image,

                })
                .ToListAsync();

            return Ok(products);
        }

        // ShopController.cs
        [HttpPut("HideProduct/{id}")]
        public IActionResult HideProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.IsVisible = false; // Hide the product
            _context.SaveChanges();

            return Ok(new { message = "Product hidden successfully" });
        }

        // API to show a product
        [HttpPut("ShowProduct/{id}")]
        public IActionResult ShowProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound("Product not found");
            }

            product.IsVisible = true; // Show the product
            _context.SaveChanges();

            return Ok(new { message = "Product shown successfully" });
        }

        // API to get only visible products for the user shop page
        [HttpGet("GetVisibleProducts")]
        public IActionResult GetVisibleProducts()
        {
            var products = _context.Products
                .Where(p => p.IsVisible) // Only select visible products
                .Select(p => new
                {
                    p.ProductId,
                    p.Name,
                    p.Image,
                    p.Price,
                    p.Amount
                })
                .ToList();

            return Ok(products);
        }

        [HttpDelete("DeletePricingOption/{shopId}")]
        public async Task<IActionResult> DeletePricingOption(int shopId)
        {
            var pricingOption = await _context.Shops.FirstOrDefaultAsync(s => s.ShopId == shopId);
            if (pricingOption == null)
            {
                Console.WriteLine($"Pricing option with shopId {shopId} not found in the database.");
                return NotFound($"Pricing option with shopId {shopId} not found.");
            }

            // Remove the pricing option
            _context.Shops.Remove(pricingOption);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Pricing option deleted successfully." });
        }





    }
}

