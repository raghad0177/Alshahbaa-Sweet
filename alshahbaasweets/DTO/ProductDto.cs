using System;

namespace alshahbaasweets.DTO
{
    public class ProductDto
    {
        public string? Name { get; set; }
        public IFormFile? Image { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public string? Amount { get; set; }
        public int CategoryId { get; set; }

    }
}
