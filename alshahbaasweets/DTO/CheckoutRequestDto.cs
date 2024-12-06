namespace alshahbaasweets.DTO
{
    public class CheckoutRequestDto
    {
        public int? UserId { get; set; }
        public decimal Amount { get; set; }
        public double DeliveryCost { get; set; }
        public string? Name { get; set; }
        public string? Branch { get; set; }
        public string? NearestBranch { get; set; }

        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public double CustomerLatitude { get; set; }
        public double CustomerLongitude { get; set; }
        public string RegionName { get; set; }
        public string OrderType { get; set; }

        public double DistanceToBranch { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int ShopId { get; set; } // Add ShopId
    }
}
