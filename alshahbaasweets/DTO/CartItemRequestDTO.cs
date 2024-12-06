namespace alshahbaasweets.DTO
{
    public class CartItemRequestDTO
    {
        public int? ProductId { get; set; }

        public int? UserId { get; set; }

        public int CartItemId { get; set; }

        public int? Quantity { get; set; }

        public int QuantityChange { get; set; }
    }
}
