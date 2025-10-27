namespace Catalog.Products.Features.GetProductById;

public class GetProductByIdHandler(ICatalogUnitOfWork unitOfWork) : IQueryHandle<GetProductByIdQuery, GetProductByIdResult>
{
    public async Task<GetProductByIdResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Products.GetByIdAsync(query.Id, cancellationToken);

        if (product is null)
        {
            throw new ProductNotFoundException(query.Id);
        }

        var productDto = product.Adapt<ProductDto>();

        return new GetProductByIdResult(productDto);
    }
}