namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(PaginationRequest PaginationRequest): IQuery<GetProductsResul>;

public record GetProductsResul(PaginatedResult<ProductDto> Products);

public class GetProductsHandler(ICatalogUnitOfWork unitOfWork) :IQueryHandle<GetProductsQuery, GetProductsResul>
{
    public async Task<GetProductsResul> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        
        var (products, totalItems) = await unitOfWork.Products.GetProductsAsync(
            pageIndex,
            pageSize,
            null,
            null,
            cancellationToken);
        
        var productDtos = products.ToList().Adapt<List<ProductDto>>();
        
        return new GetProductsResul(new PaginatedResult<ProductDto>(pageIndex, 
                                                                    pageSize,
                                                                    totalItems, 
                                                                    productDtos));
    }
}