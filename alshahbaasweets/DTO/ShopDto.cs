namespace alshahbaasweets.DTO
{
    public class ShopDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; } // For file upload
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public string? Amount { get; set; }
    }
}
