namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool Success);

public class DeleteProductHandle(CatalogDbContext dbContext) :ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await dbContext.Products.FindAsync([command], cancellationToken);

        if (product is null)
        {
            throw new Exception($"Product with Id {command} not found.");
        }

        dbContext.Products.Remove(product);
        
       await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}