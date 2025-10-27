namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid ProductId) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required.");
    }
}

public class DeleteProductHandle(ICatalogUnitOfWork unitOfWork) :ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.ProductId);
        }
        
        unitOfWork.Products.Remove(product);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new DeleteProductResult(true);
    }
}