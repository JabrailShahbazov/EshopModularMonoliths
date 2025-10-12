namespace Catalog.Products.Features.GetProducts;

public record GetProductsResponse(IEnumerable<ProductDto> Products);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async (ISender sender) =>
        {
            var products = await sender.Send(new GetProductsQuery());
            
            var response = products.Adapt<GetProductsResponse>();
            
            return Results.Ok(response);
        }).WithName("GetProducts")
          .Produces<GetProductsResponse>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .WithSummary("Gets all products.")
          .WithDescription("Gets a list of all products in the catalog.");
    }
}