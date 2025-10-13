
using Microsoft.Extensions.Logging;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product) : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(x => x.Product.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
        RuleFor(x => x.Product.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(x => x.Product.Category).NotEmpty().WithMessage("Category is required");
    }
}

public class CreateProductHandle(CatalogDbContext dbContext, 
                                 IValidator<CreateProductCommand> validator, 
                                 ILogger<CreateProductHandle> logger) : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        
        var result = await validator.ValidateAsync(command, cancellationToken);
        var errors = result.Errors.Select(x => x.ErrorMessage).ToList();

        if (errors.Count != 0)
        {
            throw new ValidationException(errors.FirstOrDefault());
        }
        
        logger.LogInformation("CreateProductCommandHandle handle called with {@command}", command);
        
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