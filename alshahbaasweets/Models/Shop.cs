using System;
using System.Collections.Generic;

namespace alshahbaasweets.Models;

public partial class Shop
{
    public int ShopId { get; set; }

    public int ProductId { get; set; }
    public bool IsVisible { get; set; } = true; // New property to track visibility

    public decimal? Price { get; set; }

  public string? Amount { get; set; }

    public virtual Product Product { get; set; } = null!;
}
