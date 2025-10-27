namespace Catalog.Products.Features.GetProductByCategory;

public record GetProductByCategoryQuery(string Category) : IQuery<GetProductByCategoryResult>;

public record GetProductByCategoryResult(IEnumerable<ProductDto> Products);

public class GetProductByCategoryHandler(ICatalogUnitOfWork unitOfWork) : IQueryHandle<GetProductByCategoryQuery, GetProductByCategoryResult>
{
    public async Task<GetProductByCategoryResult> Handle(GetProductByCategoryQuery query, CancellationToken cancellationToken)
    {
        var (products, _) = await unitOfWork.Products.GetProductsByCategoryAsync(
            query.Category,
            pageIndex: 0,
            pageSize: int.MaxValue,
            cancellationToken);
        
        var productDtos = products.ToList().Adapt<List<ProductDto>>();

        return new GetProductByCategoryResult(productDtos);
    }
}