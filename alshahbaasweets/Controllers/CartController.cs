using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alshahbaasweets.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using alshahbaasweets.DTO;

namespace alshahbaasweets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public CartController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/Cart/GetAllCartItems
        [HttpGet("GetAllCartItems")]
        public IActionResult GetAllCartItems()
        {
            Console.WriteLine("Fetching all cart items...");

            var cartItems = _dbContext.CartItems
                .Include(c => c.Product)
                .Select(c => new
                {
                    CartItemId = c.CartItemId,
                    ProductId = c.ProductId,
                    UserId = c.UserId,
                    Quantity = c.Quantity,
                    Product = new
                    {
                        Name = c.Product.Name,
                        Description = c.Product.Description,
                        Image = c.Product.Image,
                        Price = c.Product.Price
                    }
                })
                .ToList();

            if (!cartItems.Any())
            {
                Console.WriteLine("No cart items found.");
                return NotFound("No cart items found.");
            }

            Console.WriteLine("Cart items fetched successfully.");
            return Ok(cartItems);
        }

        // GET: api/Cart/GetAllCartItemsByUserId/{userId}
        [HttpGet("GetAllCartItemsByUserId/{userId:int}")]
        public IActionResult GetCartItemsByUserId(int userId)
        {
            Console.WriteLine($"Fetching cart items for user ID: {userId}");

            if (userId <= 0)
            {
                Console.WriteLine("Invalid user ID.");
                return BadRequest("Invalid user ID.");
            }

            var cartItems = _dbContext.CartItems
                .Where(c => c.UserId == userId)
                .Include(c => c.Product)
                .Select(c => new
                {
                    CartItemId = c.CartItemId,
                    ProductId = c.ProductId,
                    UserId = c.UserId,
                    Quantity = c.Quantity,
                    Product = new
                    {
                        Name = c.Product.Name,
                        Description = c.Product.Description,
                        Image = c.Product.Image,
                        Price = c.Product.Price
                    }
                })
                .ToList();

            if (!cartItems.Any())
            {
                Console.WriteLine($"No cart items found for user with ID {userId}.");
                return NotFound($"No cart items found for user with ID {userId}.");
            }

            Console.WriteLine("Cart items fetched successfully for user ID: " + userId);
            return Ok(cartItems);
        }

        // POST: api/Cart/CreateNewCartItem
        [HttpPost("CreateNewCartItem")]
        public IActionResult CreateNewCartItem([FromBody] CartItemRequestDTO cartItemDTO)
        {
            Console.WriteLine("Attempting to create a new cart item...");

            if (cartItemDTO == null || cartItemDTO.ProductId == null)
            {
                Console.WriteLine("Invalid data. Product ID is required.");
                return BadRequest("Invalid data. Product ID is required.");
            }

            var productExists = _dbContext.Products.Any(p => p.ProductId == cartItemDTO.ProductId);
            if (!productExists)
            {
                Console.WriteLine("Product does not exist.");
                return NotFound("Product does not exist.");
            }

            try
            {
                var existingCartItem = _dbContext.CartItems
                    .FirstOrDefault(c => c.UserId == cartItemDTO.UserId && c.ProductId == cartItemDTO.ProductId);

                if (existingCartItem != null)
                {
                    Console.WriteLine("Item already exists in the cart. Updating quantity...");
                    existingCartItem.Quantity += cartItemDTO.Quantity;
                    _dbContext.CartItems.Update(existingCartItem);
                    _dbContext.SaveChanges();

                    Console.WriteLine("Item quantity updated successfully.");
                    return Ok(new { item = existingCartItem, message = "Item quantity updated." });
                }

                Console.WriteLine("Adding new item to the cart...");
                var newCartItem = new CartItem
                {
                    ProductId = cartItemDTO.ProductId,
                    UserId = cartItemDTO.UserId,
                    Quantity = cartItemDTO.Quantity
                };

                _dbContext.CartItems.Add(newCartItem);
                _dbContext.SaveChanges();

                Console.WriteLine("New item added to the cart successfully.");
                return Ok(new { item = newCartItem, message = "New item added to the cart." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, $"An error occurred while adding the item to the cart: {ex.Message}");
            }
        }

        // PUT: api/Cart/UpdateCartItem
        [HttpPut("UpdateCartItem")]
        public IActionResult UpdateCartItem([FromBody] CartItemRequestDTO updateRequest)
        {
            Console.WriteLine("Attempting to update a cart item...");

            if (updateRequest == null)
            {
                Console.WriteLine("Invalid request data.");
                return BadRequest("Invalid request data.");
            }

            var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.CartItemId == updateRequest.CartItemId);
            if (cartItem == null)
            {
                Console.WriteLine("Cart item not found.");
                return NotFound("Cart item not found.");
            }

            cartItem.Quantity += updateRequest.QuantityChange;
            if (cartItem.Quantity <= 0)
            {
                Console.WriteLine("Quantity is zero or less. Removing cart item...");
                _dbContext.CartItems.Remove(cartItem);
            }
            else
            {
                Console.WriteLine("Updating cart item quantity...");
                _dbContext.CartItems.Update(cartItem);
            }

            _dbContext.SaveChanges();
            Console.WriteLine("Cart updated successfully.");
            return Ok(new { success = true, message = "Cart updated successfully." });
        }

        // DELETE: api/Cart/DeleteCartItem/{cartItemId}
        [HttpDelete("DeleteCartItem/{cartItemId:int}")]
        public IActionResult DeleteCartItem(int cartItemId)
        {
            Console.WriteLine($"Attempting to delete cart item with ID: {cartItemId}");

            if (cartItemId <= 0)
            {
                Console.WriteLine("Invalid CartItem ID.");
                return BadRequest("Invalid CartItem ID.");
            }

            var cartItem = _dbContext.CartItems.FirstOrDefault(c => c.CartItemId == cartItemId);
            if (cartItem == null)
            {
                Console.WriteLine("Cart item not found.");
                return NotFound("Cart item not found.");
            }

            _dbContext.CartItems.Remove(cartItem);
            _dbContext.SaveChanges();

            Console.WriteLine("Cart item deleted successfully.");
            return Ok(new { message = "Cart item deleted successfully." });
        }

        // DELETE: api/Cart/ClearCartByUserId/{userId}
        [HttpDelete("ClearCartByUserId/{userId:int}")]
        public IActionResult ClearCartByUserId(int userId)
        {
            Console.WriteLine($"Attempting to clear cart items for user ID: {userId}");

            if (userId <= 0)
            {
                Console.WriteLine("Invalid user ID.");
                return BadRequest("Invalid user ID.");
            }

            var cartItems = _dbContext.CartItems.Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any())
            {
                Console.WriteLine("No cart items found for the user.");
                return NotFound("No cart items found for the user.");
            }

            _dbContext.CartItems.RemoveRange(cartItems);
            _dbContext.SaveChanges();

            Console.WriteLine("All cart items cleared successfully.");
            return Ok("All cart items cleared successfully.");
        }

        // GET: api/Cart/GetCartItemCount/{userId}
        [HttpGet("GetCartItemCount/{userId}")]
        public ActionResult<int> GetCartItemCount(int userId)
        {
            Console.WriteLine($"Fetching cart item count for user ID: {userId}");

            var itemCount = _dbContext.CartItems
                .Where(c => c.UserId == userId)
                .Sum(c => c.Quantity);

            Console.WriteLine($"Cart item count for user ID {userId}: {itemCount}");
            return Ok(itemCount);
        }
    }
}
