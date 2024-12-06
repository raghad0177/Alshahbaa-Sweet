using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using alshahbaasweets.Models;
using alshahbaasweets.DTO; // Assuming you have this namespace for your DTOs
using Microsoft.AspNetCore.SignalR;
using alshahbaasweets.Services;
using MimeKit;
using MailKit.Net.Smtp;
using MimeKit;


namespace alshahbaasweets.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration; // Inject IConfiguration
        private readonly MyDbContext _context;
        private readonly EmailService _emailService; // Inject the EmailService
        private readonly IHubContext<NotificationsHub> _hubContext; // Inject HubContext
        public OrderController(MyDbContext context, EmailService emailService, IHubContext<NotificationsHub> hubContext, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;

            _hubContext = hubContext;
        }

        [HttpGet("ApplyCoupon")]
        public ActionResult<decimal> ApplyCoupon(string couponName, decimal orderAmount, decimal deliveryFee)
        {
            // Ensure the couponName is not null or empty
            if (string.IsNullOrEmpty(couponName))
            {
                return Ok(orderAmount + deliveryFee); // Return the original amount if coupon is invalid
            }

            // Find the coupon, making sure it is active (Status = 1)
            var coupon = _context.Copons.FirstOrDefault(c => c.Name == couponName && c.Status == 1);

            if (coupon == null)
            {
                return Ok(orderAmount + deliveryFee); // If no valid coupon, return the total without discount
            }

            decimal discountAmount = 0;

            // Calculate the discount based on the coupon type
            switch (coupon.DiscountType)
            {
                case "FixedAmount":
                    discountAmount = coupon.DiscountValue;
                    break;
                case "PercentageOnOrder":
                    discountAmount = orderAmount * (coupon.DiscountValue / 100);
                    break;
                case "PercentageOnDelivery":
                    discountAmount = deliveryFee * (coupon.DiscountValue / 100);
                    break;
            }

            // Calculate the new total after applying the discount
            decimal newTotal = (orderAmount + deliveryFee) - discountAmount;

            // Create an Order object to show how it would look with the coupon applied
            var order = new Order
            {
                Amount = newTotal, // Set the discounted amount
                CoponId = coupon.CoponId, // Associate the coupon with the order
                DeliveryCost = (double)deliveryFee, // Use the provided delivery fee
                Date = DateTime.Now, // Set the date to today
                Status = "Pending", // Example status, update as needed
                RegionName = "Sample Region", // Example, update with actual data
                CustomerLatitude = 0.0, // Example value, update as needed
                CustomerLongitude = 0.0, // Example value, update as needed
                DistanceToBranch = 0.0 // Example value, update as needed
            };

            // You may want to save this order to the database if necessary

            // Return the new total and other details
            return Ok(new
            {
                message = "Coupon applied successfully",
                newTotal = newTotal,
                orderDetails = order
            });
        }



        [HttpPost("CreateCopoun")]
        public IActionResult Create([FromBody] CreateCoponDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create a new Copon object using the data from the DTO
            var copon = new Copon
            {
                Name = model.Name,
                DiscountType = model.DiscountType,
                DiscountValue = model.DiscountValue,
                Status = model.Status == 1 ? 1 : 0, // Set to 1 if active, 0 otherwise
                Date = model.Date.HasValue ? DateOnly.FromDateTime(model.Date.Value) : null // Convert DateTime to DateOnly
            };

            // Add the Copon to the database and save changes
            _context.Copons.Add(copon);
            _context.SaveChanges();

            // Return a success response with the created Copon details
            return Ok(new { message = "Copon created successfully", copon });
        }


        [HttpPost("Checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequestDto request)
        {
            if (request.UserId == null)
            {
                return BadRequest(new { message = "You must register or login first." });
            }

            // Retrieve the user from the database using the UserId
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return BadRequest(new { message = "User not found." });
            }

            // Create the order object
            var order = new Order
            {
                UserId = request.UserId,
                Amount = request.Amount,
                Branch=request.Branch,
                DeliveryCost = request.DeliveryCost,
                Status = "Pending",
                NearestBranch = request.NearestBranch,
                Date = DateTime.Now,
                Name = user.Name,
                Address = request.Address,
                PhoneNumber = user.PhoneNumber,
                CustomerLatitude = request.CustomerLatitude,
                CustomerLongitude = request.CustomerLongitude,
                RegionName = request.RegionName,
                DistanceToBranch = request.DistanceToBranch,
                OrderType = request.OrderType
            };

            // Add order items
            foreach (var item in request.OrderItems)
            {
                var shop = _context.Shops.FirstOrDefault(s => s.ShopId == item.ShopId && s.ProductId == item.ProductId);
                if (shop == null)
                {
                    return BadRequest(new { message = $"Shop or product not found for ShopId: {item.ShopId}, ProductId: {item.ProductId}" });
                }

                var orderItem = new OrderItem
                {
                    ProductId = item.ProductId,
                    ShopId = item.ShopId, // Associate the specific shop
                    Quantity = item.Quantity
                };

                order.OrderItems.Add(orderItem);
            }

            // Save the order to the database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Notify all connected clients of a new order via SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveNewOrderNotification", order);

            // Send an email to the admin
            try
            {
                // Get the admin email from configuration
                string adminEmail = _configuration["EmailSettings:AdminEmail"];
                string subject = "New Order Received";
                string itemsDetails = "";

                // Construct the order items details
                foreach (var item in order.OrderItems)
                {
                    // Assuming you have a method or property to get the item name from the ProductId
                    var product = _context.Products.Find(item.ProductId);
                    var shopItem = _context.Shops.FirstOrDefault(s => s.ProductId == item.ProductId);
                    string productName = product != null ? product.Name : "Unknown Product";
                    string shopAmount = shopItem != null ? shopItem.Amount : "Unknown Amount";

                    itemsDetails += $@"
            <li>
                 <strong>Product:</strong> {productName} <br>
                <strong>Quantity:</strong> {item.Quantity} <br>
                <strong>Amount:</strong> {shopAmount} <br>
                
            </li>";
                }
                string message = $@"
            <html>
        <body>
            <h2>New Order Details</h2>
            <p><strong>Order ID:</strong> {order.OrderId}</p>
            <p><strong>Username:</strong> {order.Name}</p> <!-- Display the user's name -->
            <p><strong>Total Amount:</strong> {order.Amount} JD</p>
            <p><strong>Delivery Cost:</strong> {order.DeliveryCost} JD</p>
            <p><strong>Address:</strong> {order.Address}</p>
            <p><strong>Phone:</strong> {order.PhoneNumber}</p>
            <p><strong>Items:</strong> {order.OrderItems.Count} item(s)</p>
            <ul>{itemsDetails}</ul> <!-- List the items with names and quantities -->
        </body>
        </html>";

                // Send the email using the EmailService
                await _emailService.SendEmailAsync(adminEmail, subject, message);

                // Log a success message if the email was sent
                Console.WriteLine("Email sent successfully to admin.");
            }
            catch (Exception ex)
            {
                // Log the error if the email fails to send
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            // Return a success response
            return Ok(new { message = "Order created successfully!", order });
        }


        // Other methods...



        [HttpPut("ChangeCouponStatus/{id}")]
        public IActionResult ChangeCouponStatus(int id)
        {
            // Find the coupon by ID
            var coupon = _context.Copons.Find(id);
            if (coupon == null)
            {
                return NotFound("Coupon not found.");
            }

            // Toggle the status: if 1, set to 0; if 0, set to 1
            coupon.Status = coupon.Status == 1 ? 0 : 1;

            // Save changes to the database
            _context.SaveChanges();

            // Debugging: Log the new status
            Console.WriteLine($"Coupon ID: {id} status changed to: {coupon.Status}");

            // Return the updated coupon
            return Ok(new { message = "Coupon status updated successfully", coupon });
        }








        [HttpGet("GetAllCoupons")]
        public IActionResult GetAllCoupons()
        {
            // Fetch all coupons from the database
            var coupons = _context.Copons.ToList();

            // Debugging: Log the fetched coupons
            Console.WriteLine("Coupons fetched from database:");
            foreach (var coupon in coupons)
            {
                Console.WriteLine($"ID: {coupon.CoponId}, Name: {coupon.Name}, Status: {coupon.Status}");
            }

            // Return the list of coupons
            return Ok(coupons);
        }




        // Endpoint to check coupon validity
        [HttpGet("GetCoupunByName/{name}")]
        public IActionResult GetCoupunByName(string name)
        {

            var coupon = _context.Copons
                .Where(c => c.Name == name)
                .Select(c => new CouponResponseDTO
                {
                    Name = c.Name,
                    DiscountValue = c.DiscountValue,
                    Date = c.Date,
                    Status = c.Status,
                })
                .FirstOrDefault();

            var coponnew = _context.Copons.Where(c => c.Name == name).FirstOrDefault();

            if (coupon == null)
            {
                return NotFound(new { message = $"No coupon named {name} found" });
            }

            if (coupon.Date.HasValue && coupon.Date.Value > DateOnly.FromDateTime(DateTime.Today))
            {
                return Ok(coponnew);
            }
            else
            {
                return BadRequest(new { message = "Coupon is expired or not valid" });
            }
        }


        [HttpPost("Register")]
        public IActionResult Register([FromBody] UserRegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a user with the same phone number already exists
            var existingUser = _context.Users.FirstOrDefault(u => u.PhoneNumber == model.PhoneNumber);
            if (existingUser != null)
            {
                return BadRequest(new { message = "رقم الهاتف مسجل بالفعل." });
            }

            // Create a new user object
            var newUser = new User
            {
                Name = model.Name,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate
            };

            // Save the user to the database
            _context.Users.Add(newUser);
            _context.SaveChanges();

            // Automatically log the user in by returning the user ID
            return Ok(new { userId = newUser.UserId, message = "تم التسجيل بنجاح" });
        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequestDto loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.PhoneNumber))
            {
                return BadRequest(new { message = "رقم الهاتف مطلوب." });
            }

            // Check if a user with the provided phone number exists in the database
            var user = _context.Users.FirstOrDefault(u => u.PhoneNumber == loginRequest.PhoneNumber);

            if (user == null)
            {
                // If no user is found, return an error
                return NotFound(new { message = "فشل تسجيل الدخول. لم يتم العثور على مستخدم برقم الهاتف هذا." });
            }

            // If user is found, return the user ID
            return Ok(new { userId = user.UserId, message = "تم تسجيل الدخول بنجاح." });
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUser(int userId)
        {
            // Retrieve the user from the database
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                // If the user is not found, return a 404 response
                return NotFound(new { message = "User not found" });
            }

            // Return the user details
            return Ok(new
            {
                userId = user.UserId,
                name = user.Name,
                phoneNumber = user.PhoneNumber,
                address = user.Address
            });
        }

        [HttpGet]
        [Route("AllOrders")]
        public IActionResult GetAllOrders()
        {
            var orders = _context.Orders
                .Select(order => new OrderResponseDto
                {
                    OrderId = order.OrderId,
                    Date = order.Date,

                    // Customer information
                    Customer = new UserDto
                    {
                        Name = order.User.Name
                    },

                    // Calculate the total number of items
                    NumberOfItems = order.OrderItems.Sum(oi => oi.Quantity ?? 0),

                    // Calculate the total price of the order
                    Total = order.Amount,

                    // Status of the order
                    Status = order.Status,

                    // Map each order item to OrderItemDto
                    OrderItems = order.OrderItems.Select(oi => new OrderItemsDto
                    {
                        ProductId = oi.Product.ProductId,
                        ProductName = oi.Product.Name,
                        Quantity = oi.Quantity,
                        Price = oi.Product.Amount
                    }).ToList()
                })
                .ToList();

            return Ok(orders);
        }

        [HttpGet("GetOrdersWithProductsByUser/{userId}")]
        public IActionResult GetOrdersWithProductsByUser(int userId)
        {
            var orders = _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    OrderId = o.OrderId,
                    Products = o.OrderItems.Select(oi => new
                    {
                        ProductId = oi.ProductId,
                        ProductName = oi.Product.Name,
                        OrderItemId = oi.OrderItemId
                    }).ToList()
                }).ToList();

            if (!orders.Any())
            {
                return NotFound("No orders found for this user.");
            }

            return Ok(orders);
        }

        //[HttpGet("GetOrderDetails/{orderId}")]
        //public IActionResult GetOrderDetails(int orderId)
        //{
        //    // Retrieve the order from the database
        //    var order = _context.Orders
        //        .Include(o => o.User) // Include the user information
        //        .Include(o => o.OrderItems) // Include order items
        //        .ThenInclude(oi => oi.Product) // Include the product details for each order item
        //        .FirstOrDefault(o => o.OrderId == orderId);

        //    if (order == null)
        //    {
        //        return NotFound(new { message = "Order not found" });
        //    }

        //    // Construct the response DTO with all details
        //    var orderDetails = new
        //    {
        //        OrderId = order.OrderId,
        //        User = new
        //        {
        //            UserId = order.User?.UserId,
        //            Name = order.User?.Name,
        //            PhoneNumber = order.User?.PhoneNumber,
        //        },
        //        Amount = order.Amount,
        //        DeliveryCost = order.DeliveryCost,
        //        Status = order.Status,
        //        TransactionId = order.TransactionId,
        //        Date = order.Date,
        //        Name = order.Name,
        //        Address = order.Address,
        //        PhoneNumber = order.PhoneNumber,
        //        CustomerLatitude = order.CustomerLatitude,
        //        CustomerLongitude = order.CustomerLongitude,
        //        RegionName = order.RegionName,
        //        DistanceToBranch = order.DistanceToBranch,
        //        Items = order.OrderItems.Select(oi => new
        //        {
        //            OrderItemId = oi.OrderItemId,
        //            ProductId = oi.ProductId,
        //            ProductName = oi.Product?.Name,
        //            Quantity = oi.Quantity,
        //            Price = oi.Product?.Price,
        //        }).ToList()
        //    };

        //    return Ok(orderDetails);
        //}


        [HttpGet("GetOrderDetails/{orderId}")]
        public IActionResult GetOrderDetails(int orderId)
        {
            // Retrieve the order from the database
            var order = _context.Orders
                .Include(o => o.User) // Include the user information
                .Include(o => o.OrderItems) // Include order items
                .ThenInclude(oi => oi.Product) // Include the product details for each order item
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Construct the response DTO with all details
            var orderDetails = new
            {
                OrderId = order.OrderId,
                User = new
                {
                    UserId = order.User?.UserId,
                    Name = order.User?.Name,
                    PhoneNumber = order.User?.PhoneNumber,
                },
                Amount = order.Amount,
                DeliveryCost = order.DeliveryCost,
                Branch = order.Branch,
                Status = order.Status,
                NearestBranch = order.NearestBranch,
                Date = order.Date,
                Name = order.Name,
                Address = order.Address,
                PhoneNumber = order.PhoneNumber,
                CustomerLatitude = order.CustomerLatitude,
                CustomerLongitude = order.CustomerLongitude,
                RegionName = order.RegionName,
                DistanceToBranch = order.DistanceToBranch,
                Items = order.OrderItems.Select(oi => new
                {
                    OrderItemId = oi.OrderItemId,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name,
                    Quantity = oi.Quantity,
                    Price = _context.Shops
         .Where(s => s.ProductId == oi.ProductId && s.ShopId == oi.ShopId) // Match ShopId and ProductId
         .Select(s => s.Price)
         .FirstOrDefault(),
                    Amount = _context.Shops
         .Where(s => s.ProductId == oi.ProductId && s.ShopId == oi.ShopId) // Match ShopId and ProductId
         .Select(s => s.Amount)
         .FirstOrDefault(),
                }).ToList()
            };

        
            return Ok(orderDetails);
        }

        [HttpPut("UpdateOrderStatus/{orderId}")]
        public IActionResult UpdateOrderStatus(int orderId, [FromBody] OrderStatusUpdateDto statusUpdate)
        {
            // Find the order by ID
            var order = _context.Orders.Find(orderId);
            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            // Update the status

            string statusString = statusUpdate.Status == 0 ? "Pending" : "Completed";

            order.Status = statusString;

            // Save changes to the database
            _context.SaveChanges();

            return Ok(new { message = "Order status updated successfully", order });
        }
    }

    // DTO for updating order status
    public class OrderStatusUpdateDto
    {
        public int Status { get; set; } // 0 for Pending, 1 for Completed
    }
}




