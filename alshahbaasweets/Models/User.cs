using System;
using System.Collections.Generic;

namespace alshahbaasweets.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; } 

    public string? Address { get; set; }
    public string? Password { get; set; }

    public byte[]? Password_hash { get; set; }                

    public byte[]? Password_salt { get; set; }
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
