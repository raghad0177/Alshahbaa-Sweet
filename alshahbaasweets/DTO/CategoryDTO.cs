namespace alshahbaasweets.DTO
{
    public class CategoryDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; } // For handling image file upload
    }
}
