namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(): IQuery<GetProductsResul>;

public record GetProductsResul(IEnumerable<ProductDto> Products);

public class GetProductsHandler(CatalogDbContext dbContext) :IQueryHandle<GetProductsQuery, GetProductsResul>
{
    public async Task<GetProductsResul> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var products =await dbContext.Products
                                .AsNoTracking()
                                .OrderBy(p => p.Name)
                                .ToListAsync(cancellationToken);
        
        // var productDtos = await dbContext.Products
        //     .AsNoTracking()
        //     .OrderBy(p => p.Name)
        //     .ProjectToType<ProductDto>() // Mapster-in EF üçün extension-u
        //     .ToListAsync(cancellationToken);

        var productDtos = products.Adapt<List<ProductDto>>();
        
        return new GetProductsResul(productDtos);
    }
}