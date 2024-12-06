using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace alshahbaasweets.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? UserId { get; set; }

    public decimal Amount { get; set; }

    public int? CoponId { get; set; }

    public string? Status { get; set; }

    public string? NearestBranch { get; set; }

    public DateTime Date { get; set; }

    public string? Name { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Branch { get; set; }


    [Required]
    public double CustomerLatitude { get; set; }

    [Required]
    public double CustomerLongitude { get; set; }

    [Required]
    public double DeliveryCost { get; set; }

    public string RegionName { get; set; } // Field to store the region name

    public double DistanceToBranch { get; set; }
    public string OrderType { get; set; } // Field to store the region name


    public virtual Copon? Copon { get; set; }

 public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual User? User { get; set; }
}
