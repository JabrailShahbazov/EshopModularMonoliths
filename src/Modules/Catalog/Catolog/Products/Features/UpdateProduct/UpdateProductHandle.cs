namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto Product) : ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}

public class UpdateProductHandle(ICatalogUnitOfWork unitOfWork) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(command.Product.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(command.Product.Id);
        }
        
        UpdateProductWithNewValues(command.Product, product);
        
        unitOfWork.Products.Update(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
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