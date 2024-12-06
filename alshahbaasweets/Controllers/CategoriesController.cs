using alshahbaasweets.DTO;
using alshahbaasweets.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace alshahbaasweets.Controllers
{
	[Route("api/categories")] // Base route for the CategoriesController
	[ApiController]
	public class CategoriesController : ControllerBase
	{
		private readonly MyDbContext _context;

		public CategoriesController(MyDbContext context)
		{
			_context = context;
		}

		// GET: api/categories
		[HttpGet]
		public IActionResult GetAllCategories()
		{
			var categories = _context.Categories.ToList();

			// Debug: Log the data being returned
			Console.WriteLine("Categories fetched from database:");
			foreach (var category in categories)
			{
				Console.WriteLine($"ID: {category.CategoryId}, Name: {category.Name}, Description: {category.Description}");
			}

			return Ok(categories);
		}

		// GET: api/categories/{id}
		[HttpGet("{id:int}")] // Specify constraint: id must be an integer
		public IActionResult GetCategoryById(int id)
		{
			if (id <= 0)
			{
				return BadRequest("ID must be greater than 0.");
			}

			var category = _context.Categories.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			return Ok(category);
		}

		// GET: api/categories/name/{name}
		[HttpGet("name/{name}")]
		public IActionResult GetCategoryByName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return BadRequest("Name cannot be null or empty.");
			}

			var category = _context.Categories.FirstOrDefault(c => c.Name == name);
			if (category == null)
			{
				return NotFound();
			}

			return Ok(category);
		}

		// DELETE: api/categories/{id}
		[HttpDelete("{id:int}")] // Specify constraint: id must be an integer
		public IActionResult DeleteCategory(int id)
		{
			if (id <= 0)
			{
				return BadRequest("ID must be greater than 0.");
			}

			var category = _context.Categories.Find(id);
			if (category == null)
			{
				return NotFound();
			}

			_context.Categories.Remove(category);
			_context.SaveChanges();

			return NoContent();
		}

		// POST: api/categories
		[HttpPost]
		public async Task<IActionResult> AddCategory([FromForm] CategoryDTO categoryDto, IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return BadRequest("No image file uploaded.");
			}

			// Generate a unique file name to avoid conflicts
			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
			string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images", fileName);

			// Ensure the upload directory exists
			var directory = Path.GetDirectoryName(filePath);
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			// Save the file to the server
			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			// Create the category with the image URL
			var category = new Category
			{
				Name = categoryDto.Name,
				Description = categoryDto.Description,
				Image = "/uploads/images/" + fileName  // Store the relative file path in the database
			};

			_context.Categories.Add(category);
			await _context.SaveChangesAsync();

			return Ok(category);
		}


		[HttpPut("{id:int}")] // Specify constraint: id must be an integer
		public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryDTO dto)
		{
			var category = _context.Categories.Find(id);
			if (category == null) return NotFound("Category not found.");

			// Debug logs
			Console.WriteLine($"Updating category with ID: {id}");
			Console.WriteLine($"Received Name: {dto.Name}");
			Console.WriteLine($"Received Description: {dto.Description}");

			// Update properties
			category.Name = dto.Name;
			category.Description = dto.Description;

			// Handle image upload
			if (dto.Image != null)
			{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images", fileName);

				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					await dto.Image.CopyToAsync(fileStream);
				}

				category.Image = "/uploads/images/" + fileName;
			}

			_context.SaveChanges();
			Console.WriteLine("Category updated successfully.");
			return Ok(category);
		}
	}
}
