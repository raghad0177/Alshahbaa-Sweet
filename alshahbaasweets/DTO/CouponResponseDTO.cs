namespace alshahbaasweets.DTO
{
    public class CouponResponseDTO
    {
        public string? Name { get; set; }

        public decimal DiscountValue { get; set; }

        public DateOnly? Date { get; set; }

        public int? Status { get; set; }
    }
}
