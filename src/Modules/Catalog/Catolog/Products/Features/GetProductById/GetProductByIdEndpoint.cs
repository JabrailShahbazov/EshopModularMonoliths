namespace Catalog.Products.Features.GetProductById;

public record GetProductByIdResponse(ProductDto Product);

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/products/{id}", async (Guid id, ISender sender) =>
        {
            var result = await sender.Send(new GetProductByIdQuery(id));
            
            var response = result.Adapt<GetProductByIdResponse>();
            
            return Results.Ok(response);
        }).WithName("GetProductById")
          .Produces<GetProductByIdResponse>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .WithSummary("Gets an existing product by id")
          .WithDescription("Gets an existing product with the provided id.");
    }
}