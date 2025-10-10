namespace Catalog.Products.Modules;

public class Product : Aggregate<Guid>
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

        product.AddDomainEvent( new ProductCreatedEvent(product));
        
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

        if (Price != price)
        {
            AddDomainEvent(new ProductPriceChangedEvent(this));
        }
    }
}