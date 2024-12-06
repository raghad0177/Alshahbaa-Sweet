using alshahbaasweets.DTO;
using alshahbaasweets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace alshahbaasweets.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly TokenGenerator _tokenGenerator;

        public UsersController(MyDbContext db, PasswordHasher<User> passwordHasher, TokenGenerator tokenGenerator)
        {
            _db = db;
            _passwordHasher = passwordHasher; // Initialize the PasswordHasher
            _tokenGenerator = tokenGenerator; // Initialize the TokenGenerator
        }


        [HttpGet]
        public IActionResult getAllUsers()
        {
            // Fetch the users from the database
            var Users = _db.Users.ToList();

            // Check if the Users list is null or empty
            if (Users == null || !Users.Any())
            {
                return NoContent(); // Return 204 No Content if no users are found
            }

            // If data is found, return it
            return Ok(Users);
        }


        [HttpGet]
        [Route("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid ID. The ID must be a positive integer." });
            }

            var user = _db.Users.Find(id);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            return Ok(user);
        }



        [HttpPut]
        [Route("UpdateUser/{id}")]
        public IActionResult UpdateUser(int id, [FromForm] UsersRequestDTO updatedUser)
        {
            var existingUser = _db.Users.Find(id);
            if (existingUser == null)
            {
                return NotFound(new { message = $"User with ID {id} not found." });
            }

            // Update Name
            existingUser.Name = updatedUser.Name ?? existingUser.Name;

            // Update PhoneNumber
            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;

            // Update Address if provided
            existingUser.Address = updatedUser.Address ?? existingUser.Address;

            // Update Password if provided
            if (!string.IsNullOrEmpty(updatedUser.Password))
            {
                // Hash and salt the new password before saving
                var (passwordHash, passwordSalt) = HashPassword(updatedUser.Password);
                existingUser.Password_hash = passwordHash;  // Change this to Password_hash
                existingUser.Password_salt = passwordSalt;  // Change this to Password_salt
            }

            _db.Users.Update(existingUser);
            _db.SaveChanges();

            return Ok(new { message = "User updated successfully.", user = existingUser });
        }


        private (byte[] passwordHash, byte[] passwordSalt) HashPassword(string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return (passwordHash, passwordSalt);
            }
        }


        [HttpPost("registerAdmin")]
        public ActionResult RegisterAdmin( UserDTO model)
        {
            // Hash the password
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

            var user = new Admin
            {
                Name = model.UserName,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Email = model.Email
               
            };

            _db.Admins.Add(user);
            _db.SaveChanges();

            return Ok(user);
        }

        [HttpPost("registerCustomer")]
        public ActionResult RegisterCustomer(NormalUser model)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(model.Name) || string.IsNullOrEmpty(model.Email) ||
                string.IsNullOrEmpty(model.PhoneNumber) || model.BirthDate == null ||
                string.IsNullOrEmpty(model.Address) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { success = false, message = "Please fill in all required fields." });
            }

            // Check if email already exists
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { success = false, message = "The email is already taken." });
            }

            // Hash the password
            byte[] passwordHash, passwordSalt;
            PasswordHasher.CreatePasswordHash(model.Password, out passwordHash, out passwordSalt);

            var user = new User
            {
                Name = model.Name,
                Password_hash = passwordHash,
                Password_salt = passwordSalt,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                BirthDate = model.BirthDate,
                Address = model.Address
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new { success = true, message = "Registration successful!" });
        }


        [HttpPost("loginAdmin")]
        public IActionResult LoginAdmin( DTOsLogin model)
        {

            // Regular email/password login
            var user = _db.Admins.FirstOrDefault(x => x.Email == model.Email);
            if (user == null || !PasswordHasher.VerifyPasswordHash(model.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Retrieve roles and generate JWT token
            var token = _tokenGenerator.GenerateToken(user.Name);

            return Ok(new { Token = token, userID = user.AdminId });
        }
        [HttpPost("loginCustomer")]
        public IActionResult LoginCustomer( DTOsLogin model)
        {
            // Regular email/password login
            var user = _db.Users.FirstOrDefault(x => x.Email == model.Email);
            if (user == null || !PasswordHasher.VerifyPasswordHash(model.Password, user.Password_hash, user.Password_salt))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Retrieve roles and generate JWT token
            var token = _tokenGenerator.GenerateToken(user.Name);

            return Ok(new { Token = token, userID = user.UserId });
        }



    }
}