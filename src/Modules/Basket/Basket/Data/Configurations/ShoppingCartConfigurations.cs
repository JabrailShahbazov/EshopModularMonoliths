using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Basket.Data.Configurations;

public class ShoppingCartConfigurations : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(e => e.UserName).IsUnique();

        builder.Property(e => e.UserName).IsRequired().HasMaxLength(100);

        builder.HasMany(c => c.Items)
            .WithOne()
            .HasForeignKey(i => i.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}