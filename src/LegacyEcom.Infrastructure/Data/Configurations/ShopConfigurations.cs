using LegacyEcom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegacyEcom.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> b)
    {
        b.ToTable("Customers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.UserId).HasMaxLength(128);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30);
        b.HasIndex(x => x.UserId).HasDatabaseName("IX_Customers_UserId");
        b.HasIndex(x => x.Email).HasDatabaseName("IX_Customers_Email");

        b.HasMany(x => x.Addresses)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> b)
    {
        b.ToTable("Addresses");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.Label).HasMaxLength(50);
        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Street).HasMaxLength(250).IsRequired();
        b.Property(x => x.City).HasMaxLength(100).IsRequired();
        b.Property(x => x.State).HasMaxLength(100).IsRequired();
        b.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();
        b.Property(x => x.Country).HasMaxLength(100).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(30);
        b.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Addresses_CustomerId");
    }
}

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> b)
    {
        b.ToTable("CartItems");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.UserId).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.UserId).HasDatabaseName("IX_CartItems_UserId");
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.OrderNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.UserId).HasMaxLength(128);
        b.Property(x => x.OrderDate).IsRequired();
        b.Property(x => x.Status).HasMaxLength(50).IsRequired();
        b.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
        b.Property(x => x.ShippingCost).HasColumnType("decimal(18,2)");
        b.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
        b.Property(x => x.Total).HasColumnType("decimal(18,2)");
        b.Property(x => x.ShippingMethod).HasMaxLength(50).IsRequired();
        b.Property(x => x.PaymentMethod).HasMaxLength(50).IsRequired();
        b.Property(x => x.ShipFirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.ShipLastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.ShipEmail).HasMaxLength(256).IsRequired();
        b.Property(x => x.ShipPhone).HasMaxLength(30);
        b.Property(x => x.ShipStreet).HasMaxLength(250).IsRequired();
        b.Property(x => x.ShipCity).HasMaxLength(100).IsRequired();
        b.Property(x => x.ShipState).HasMaxLength(100).IsRequired();
        b.Property(x => x.ShipPostalCode).HasMaxLength(20).IsRequired();
        b.Property(x => x.ShipCountry).HasMaxLength(100).IsRequired();

        b.HasIndex(x => x.OrderNumber).IsUnique().HasDatabaseName("IX_Orders_OrderNumber");
        b.HasIndex(x => x.UserId).HasDatabaseName("IX_Orders_UserId");
        b.HasIndex(x => x.CustomerId).HasDatabaseName("IX_Orders_CustomerId");

        b.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(x => x.OrderLines)
            .WithOne(x => x.Order)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> b)
    {
        b.ToTable("OrderLines");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).UseIdentityColumn();
        b.Property(x => x.ProductName).HasMaxLength(200).IsRequired();
        b.Property(x => x.VariantName).HasMaxLength(200);
        b.Property(x => x.Sku).HasMaxLength(50);
        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        b.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");
        b.HasIndex(x => x.OrderId).HasDatabaseName("IX_OrderLines_OrderId");
    }
}
