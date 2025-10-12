namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(): IQuery<GetProductsResul>;

public record GetProductsResul(IEnumerable<ProductDto> Products);

public class GetProductsHandler(CatalogDbContext dbContext) :IQueryHandle<GetProductsQuery, GetProductsResul>
{
    public async Task<GetProductsResul> Handle(GetProductsQuery command, CancellationToken cancellationToken)
    {
        var products =await dbContext.Products
                                .AsNoTracking()
                                .OrderBy(p => p.Name)
                                .ToListAsync(cancellationToken);

        var productDtos = ProductToProductDto(products);
        
        return new GetProductsResul(productDtos);
    }

    private List<ProductDto> ProductToProductDto(List<Product> products)
    {
        // var productDtos = products.Select(p => new ProductDto
        // {
        //     Id = p.Id,
        //     Name = p.Name,
        //     Description = p.Description,
        //     Price = p.Price,
        //     CreatedAt = p.CreatedAt,
        //     UpdatedAt = p.UpdatedAt
        // }).ToList();
        //
        // return productDtos;
        
        throw new NotImplementedException();
    }
}