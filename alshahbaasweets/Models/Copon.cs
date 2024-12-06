using System;
using System.Collections.Generic;

namespace alshahbaasweets.Models
{
    public partial class Copon
    {
        public int CoponId { get; set; }

        public string? Name { get; set; }

        public decimal? Amount { get; set; }

        public DateOnly? Date { get; set; }

        public int? Status { get; set; }

        // New properties added
        public string DiscountType { get; set; } // "FixedAmount", "PercentageOnOrder", "PercentageOnDelivery"
        public decimal DiscountValue { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
