namespace alshahbaasweets.DTO
{
    public class ConfirmationViewModel
    {
        public int OrderId { get; set; }
        public double CustomerLatitude { get; set; }
        public double CustomerLongitude { get; set; }
        public double DeliveryCost { get; set; }
        public string RegionName { get; set; }
        public double DistanceToBranch { get; set; }
    }
}
