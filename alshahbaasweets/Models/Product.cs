using System;
using System.Collections.Generic;

namespace alshahbaasweets.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Image { get; set; }

    public decimal? Price { get; set; }

    public int? CategoryId { get; set; }

    public string? Amount { get; set; }


    public bool IsVisible { get; set; } = true; // New property to track visibility

    public bool CartVisible { get; set; } = true; // Default value is true (visible)

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual Category? Category { get; set; }


    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
}
