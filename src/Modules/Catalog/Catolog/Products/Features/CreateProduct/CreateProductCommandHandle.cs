using MediatR;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(string Name, string Description, decimal Price, string ImageFile, List<string> Category) : IRequest<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandHandle :IRequestHandler<CreateProductCommand, CreateProductResult>
{
    public Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}