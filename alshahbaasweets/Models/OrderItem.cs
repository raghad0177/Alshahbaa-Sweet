using System;
using System.Collections.Generic;

namespace alshahbaasweets.Models;

public partial class OrderItem
{
    public int OrderItemId { get; set; }

    public int? OrderId { get; set; }

    public int? ProductId { get; set; }
    public int? ShopId { get; set; } // Add this property


    public int? Quantity { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
    public virtual Shop? Shop { get; set; } // Navigation to Shop

}
