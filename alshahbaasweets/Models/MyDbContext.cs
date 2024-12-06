using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace alshahbaasweets.Models;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }



    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<Copon> Copons { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admin__43AA4141EEAB7919");

            entity.ToTable("Admin");

            entity.Property(e => e.AdminId).HasColumnName("admin_id");
            entity.Property(e => e.Email)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Img).HasColumnName("img");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.PasswordHash).HasColumnName("passwordHash");
            entity.Property(e => e.PasswordSalt).HasColumnName("passwordSalt");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__Cart_Ite__3C0E2A446448F495");

            entity.ToTable("Cart_Item");

            entity.Property(e => e.CartItemId).HasColumnName("Cart_Item_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Cart_Item__produ__4BAC3F29");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Cart_Item__user___4CA06362");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__D54EE9B4BAFC11A1");

            entity.ToTable("Category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        
        

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.HasKey(e => e.ContactId).HasName("PK__Contact__024E7A8668A047C4");

            entity.ToTable("Contact");

            entity.Property(e => e.ContactId).HasColumnName("contact_id");
            entity.Property(e => e.AdminResponse).HasColumnName("admin_response");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ResponseDate).HasColumnName("response_date");
            entity.Property(e => e.SentDate).HasColumnName("sent_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Sub).HasColumnName("sub");
        });

        modelBuilder.Entity<Copon>(entity =>
        {
            entity.HasKey(e => e.CoponId).HasName("PK__Copons__8A47306FE1503453");

            entity.Property(e => e.CoponId).HasColumnName("copon_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Status).HasColumnName("status");

            // Adding the new properties
            entity.Property(e => e.DiscountType)
                .HasMaxLength(50) // You can adjust the length as needed
                .HasColumnName("DiscountType");

            entity.Property(e => e.DiscountValue)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("DiscountValue");
        });


        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__Orders__465962296DEB2627");

            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CoponId).HasColumnName("copon_id");
            entity.Property(e => e.Branch).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(50);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.NearestBranch).HasColumnName("NearestBranch");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            // New fields added for geolocation and delivery details
            entity.Property(e => e.CustomerLatitude)
                .IsRequired() // Makes the property required
                .HasColumnName("CustomerLatitude")
                .HasColumnType("float"); // Using float for latitude

            entity.Property(e => e.CustomerLongitude)
                .IsRequired() // Makes the property required
                .HasColumnName("CustomerLongitude")
                .HasColumnType("float"); // Using float for longitude

            entity.Property(e => e.DeliveryCost)
                .IsRequired() // Makes the property required
                .HasColumnName("DeliveryCost")
                .HasColumnType("float"); // Using float for delivery cost

            entity.Property(e => e.RegionName)
                .HasMaxLength(255) // Optional: Specify max length for RegionName
                .HasColumnName("RegionName");

            entity.Property(e => e.DistanceToBranch)
                .HasColumnName("DistanceToBranch")
                .HasColumnType("float"); // Using float for distance

            // Foreign key relationships
            entity.HasOne(d => d.Copon).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CoponId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Orders__copon_id__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Orders__user_id__52593CB8");
        });


        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__Order_It__483A64F9A92D4DE2");

            entity.ToTable("Order_Item");

            entity.Property(e => e.OrderItemId).HasColumnName("Order_Item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Order_Ite__order__4F7CD00D");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Order_Ite__produ__5070F446");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__47027DF59E2F6394");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Image).HasColumnName("image");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");

            // Adding the IsVisible column
            entity.Property(e => e.IsVisible)
                .HasColumnName("isvisible")
                .HasDefaultValue(true); // Set default to true (visible)

            // Adding the CartVisible column
            entity.Property(e => e.CartVisible)
                .HasColumnName("cartvisible")
                .HasDefaultValue(true); // Set default to true (Add to Cart button is visible)

            // Assuming Product has a CategoryId and a navigation property to Category
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category) // Product has one Category
                .WithMany(c => c.Products) // Category has many Products
                .HasForeignKey(p => p.CategoryId) // Foreign key on Product pointing to Category
                .OnDelete(DeleteBehavior.Cascade) // Cascade delete when a Category is deleted
                .HasConstraintName("FK__Products__catego__534D60F1"); // Name of the foreign key constraint
        });



        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.ShopId).HasName("PK__Shop__AD081786C0D48CFB");

            entity.ToTable("Shop");

            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.Amount).HasColumnName("amount");

            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");

            // Adding the IsVisible column
            entity.Property(e => e.IsVisible)
                .HasColumnName("isvisible")
                .HasDefaultValue(true); // Set default to true (visible)

            entity.HasOne(d => d.Product).WithMany(p => p.Shops)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Shop__product_id__6FE99F9F");
        });


        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F8BE69844");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .IsUnicode(false)
                .HasColumnName("phone_number");

            // Add BirthDate property configuration
            entity.Property(e => e.BirthDate)
                .HasColumnName("birthdate") // Map to the "birthdate" column
                .HasColumnType("date"); // Specify that it is a DATE type
        });


        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
