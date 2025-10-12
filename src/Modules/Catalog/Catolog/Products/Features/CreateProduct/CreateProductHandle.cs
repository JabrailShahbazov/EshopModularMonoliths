namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product) : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductHandle(CatalogDbContext dbContext) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = CreateNewProduct(command.Product);
        
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new CreateProductResult(product.Id);
    }

    private Product CreateNewProduct(ProductDto productDto)
    {
       var product = Product.Create(Guid.NewGuid(),
                                    productDto.Name,
                                    productDto.Description,
                                    productDto.Price, 
                                    productDto.ImageFile, 
                                    productDto.Category);

       return product;
    }
}