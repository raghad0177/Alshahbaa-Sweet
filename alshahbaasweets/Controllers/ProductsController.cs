using alshahbaasweets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.IO;



namespace alshahbaasweets.DTO
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(MyDbContext context, ILogger<ProductsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("GetAllCartVisibilityStatus")]
        public IActionResult GetAllCartVisibilityStatus()
        {
            var products = _context.Products
                .GroupJoin(_context.Shops,
                    p => p.ProductId,
                    s => s.ProductId,
                    (p, shops) => new
                    {
                        productId = p.ProductId,
                        name = p.Name,
                        cartVisible = p.CartVisible,
                        shopIds = shops.Select(s => s.ShopId).ToList() // Collect all ShopIds for this product
                    })
                .ToList();

            return Ok(products);
        }




        [HttpPut("UpdateAllCartVisibilityStatus")]
        public IActionResult UpdateAllCartVisibilityStatus([FromBody] UpdateCartVisibilityRequest request)
        {
            // Get all products from the database
            var products = _context.Products.ToList();

            // Update the CartVisible status for each product
            foreach (var product in products)
            {
                product.CartVisible = request.CartVisible;
            }

            // Save changes to the database
            _context.SaveChanges();

            return Ok(new { message = "Cart visibility status updated for all products successfully." });
        }




        [HttpGet("{id}")]
        public IActionResult GetProductById(int id)
        {
            var product = _context.Products
                .Where(p => p.ProductId == id)
                .FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            var productDetailsDTO = new ProductDetailsDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Image = product.Image,  // Directly assigning string URL
                Price = product.Price,
               Amount=product.Amount,
                CategoryId = product.CategoryId,

            };

            return Ok(productDetailsDTO);
        }



        [HttpPost]
        public IActionResult AddProduct([FromForm] ProductDto productDto)
        {
            string fileName = null;
            if (productDto.Image != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                if (!Directory.Exists(uploads))
                {
                    Directory.CreateDirectory(uploads);
                }

                fileName = Path.GetFileName(productDto.Image.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.Image.CopyTo(stream);
                }
            }

            var product = new Product
            {
                Name = productDto.Name,
                Amount=productDto.Amount,
                Description = productDto.Description,
                Price = productDto.Price,
                CategoryId = productDto.CategoryId,
                Image = fileName != null ? "/uploads/images/" + fileName : null
            };

            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }


        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromForm] ProductDto productDto)
        {
            _logger.LogInformation("Updating product with ID {ProductId}", id); // Assuming _logger is configured

            var product = _context.Products.Find(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound();
            }

            if (productDto.Image != null)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images");
                Directory.CreateDirectory(uploads); // No need to check, create if not exists

                var fileName = Path.GetFileName(productDto.Image.FileName);
                var filePath = Path.Combine(uploads, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    productDto.Image.CopyTo(stream);
                }

                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", product.Image.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }

                product.Image = "/uploads/images/" + fileName;
            }

            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price ?? product.Price;
            product.Amount = product.Amount;// Use existing if null
            product.CategoryId = productDto.CategoryId; // Ensure this is captured

            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Product with ID {ProductId} updated successfully", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID {ProductId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }



























        [HttpGet("ByCategory/{categoryId}")]
        public ActionResult<List<Product>> GetProductsByCategory(int categoryId)
        {
            var products = _context.Products
                .Where(p => p.CategoryId == categoryId)
                .ToList();

            if (!products.Any())
            {
                return NotFound("No products found for this category.");
            }

            return Ok(products);
        }








        // Add this method to your ProductsController
        [HttpGet("ByCategoryId/{categoryId}")]
        public IActionResult GetProductsByCategoryId(int categoryId)
        {
            var products = _context.Products.Where(p => p.CategoryId == categoryId).ToList();
            if (products == null || !products.Any())
            {
                return NotFound();
            }
            return Ok(products);


        }

        










        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            _logger.LogInformation("Attempting to delete product with ID {ProductId}", id);

            var product = _context.Products.Find(id);
            if (product == null)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return NotFound();
            }

            _context.Products.Remove(product);
            try
            {
                _context.SaveChanges();
                _logger.LogInformation("Product with ID {ProductId} deleted successfully", id);
                return NoContent(); // Returns a 204 status code, indicating successful deletion without content
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID {ProductId}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }





        [HttpGet("GetProductsWithPricing")]
        public async Task<IActionResult> GetProductsWithPricing()
        {
            var products = await _context.Products
                .Select(product => new
                {
                    ProductId = product.ProductId,
                    Name = product.Name,
                    Image = product.Image,
                    PricingOptions = product.Shops.Select(shop => new
                    {
                        Amount = shop.Amount,
                        Price = shop.Price,
                        ShopId=shop.ShopId,
                    }).ToList()
                })
                .ToListAsync();

            return Ok(products);
        }

        [HttpPut("ToggleVisibility/{id}")]
        public IActionResult ToggleVisibility(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                return NotFound();
            }

            // Toggle the visibility
            product.IsVisible = !product.IsVisible;

            try
            {
                _context.SaveChanges();
                return Ok(new { message = "Visibility toggled successfully", isVisible = product.IsVisible });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
