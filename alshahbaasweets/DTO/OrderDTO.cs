namespace alshahbaasweets.DTO
{
    public class OrderDTO
    {
        public int? UserId { get; set; }
        public decimal Amount { get; set; }
        public int? CoponId { get; set; }
        public string? TransactionId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public double CustomerLat { get; set; }
        public double CustomerLng { get; set; }
        public double DeliveryCost { get; set; }
        public string RegionName { get; set; }
        public double DistanceToBranch { get; set; }
    }

    public class UserRegisterDto
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
    }


    public class LoginRequestDto
    {
        public string PhoneNumber { get; set; }
    }
}
