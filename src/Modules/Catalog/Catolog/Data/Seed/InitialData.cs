namespace Catalog.Data.Seed;

public sealed class InitialData
{
    public static IEnumerable<Product> Products => new List<Product>
    {
        Product.Create(Guid.NewGuid(), "Product 1", "Description for product 1", 10.0m,"ImageFile",["Catagory 1"]),
        Product.Create(Guid.NewGuid(), "Product 2", "Description for product 2", 20.0m,"ImageFile",["Catagory 2"]),
        Product.Create(Guid.NewGuid(), "Product 3", "Description for product 3", 30.0m,"ImageFile",["Catagory 3"])
    };
}