using Shared.DDD.Domain.Entities;

namespace Catalog.Products.Modules;

public class Product : Entity<Guid>
{
    public string Name { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public decimal Price { get; private set; } = 0!;

    public string ImageFile { get; private set; } = null!;

    public List<string> Category { get; private set; } = [];

    public static Product Create(Guid id,
                                 string name,
                                 string description,
                                 decimal price,
                                 string imageFile, 
                                 List<string> category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        var product = new Product
        {
            Id = id,
            Name = name,
            Description = description,
            Price = price,
            ImageFile = imageFile,
            Category = category
        };

        return product;
    }
    
    public void Update(string name,
                       string description,
                       decimal price,
                       string imageFile,
                       List<string> category)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

        Name = name;
        Description = description;
        Price = price;
        ImageFile = imageFile;
        Category = category;
    }
}