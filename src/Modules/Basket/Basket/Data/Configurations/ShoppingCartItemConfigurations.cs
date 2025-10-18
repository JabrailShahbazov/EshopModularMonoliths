using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Data.Configurations;

public class ShoppingCartItemConfigurations :IEntityTypeConfiguration<ShoppingCartItem>
{
    public void Configure(EntityTypeBuilder<ShoppingCartItem> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.Property(e => e.ProductId).IsRequired();
        
        builder.Property(e => e.ProductName).IsRequired().HasMaxLength(100);
        
        builder.Property(e => e.Quantity).IsRequired();
        
        builder.Property(e => e.Price).IsRequired();
        
        builder.Property(e => e.Color).HasMaxLength(50);
    }
}