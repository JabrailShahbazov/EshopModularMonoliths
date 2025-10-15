
namespace Catalog.Products.Features.GetProducts;



public record GetProductsResponse(PaginatedResult<ProductDto> Products);

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products", async ( [AsParameters] PaginationRequest request,ISender sender) =>
        {
            var products = await sender.Send(new GetProductsQuery(request));
            
            var response = products.Adapt<GetProductsResponse>();
            
            return Results.Ok(response);
        }).WithName("GetProducts")
          .Produces<GetProductsResponse>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .WithSummary("Gets all products.")
          .WithDescription("Gets a list of all products in the catalog.");
    }
}