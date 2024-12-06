namespace alshahbaasweets.DTO
{
    public class CreateCoponDto
    {
        // Name of the copon
        public string Name { get; set; }

        // Type of discount, e.g., "FixedAmount", "PercentageOnOrder", "PercentageOnDelivery"
        public string DiscountType { get; set; }

        // Value of the discount, e.g., 10 for a 10 JD discount or 15 for 15%
        public decimal DiscountValue { get; set; }

        // Status of the copon, 1 for active and 0 for inactive
        public int Status { get; set; } // Ensure the controller sets this appropriately

        // Date the copon is valid from or created on
        public DateTime? Date { get; set; } // Nullable in case it's not always provided
    }
}
