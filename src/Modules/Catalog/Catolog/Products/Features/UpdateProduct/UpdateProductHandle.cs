namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto Product) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductHandle(CatalogDbContext dbContext) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([command.Product.Id], cancellationToken);

        if (product is null)
        {
            throw new Exception($"Product with Id {command.Product.Id} not found.");
        }
        
        UpdateProductWithNewValues(command.Product, product);
        
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return new UpdateProductResult(true);
    }

    private void UpdateProductWithNewValues(ProductDto productDto, Product product)
    {
        product.Update(productDto.Name,
                       productDto.Description,
                       productDto.Price,
                       productDto.ImageFile,
                       productDto.Category);
    }
}