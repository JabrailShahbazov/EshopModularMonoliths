namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(PaginationRequest PaginationRequest): IQuery<GetProductsResul>;

public record GetProductsResul(PaginatedResult<ProductDto> Products);

public class GetProductsHandler(CatalogDbContext dbContext) :IQueryHandle<GetProductsQuery, GetProductsResul>
{
    public async Task<GetProductsResul> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        
        var totalItems = await dbContext.Products.LongCountAsync(cancellationToken);
        
        var products =await dbContext.Products
                                .AsNoTracking()
                                .OrderBy(p => p.Name)
                                .Skip(pageIndex * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);
        
        var productDtos = products.Adapt<List<ProductDto>>();
        
        // var productDtos = await dbContext.Products
        //     .AsNoTracking()
        //     .OrderBy(p => p.Name)
        //     .ProjectToType<ProductDto>() // Mapster-in EF üçün extension-u
        //     .ToListAsync(cancellationToken);

        
        return new GetProductsResul(new PaginatedResult<ProductDto>(pageIndex, 
                                                                    pageSize,
                                                                    totalItems, 
                                                                    productDtos));
    }
}