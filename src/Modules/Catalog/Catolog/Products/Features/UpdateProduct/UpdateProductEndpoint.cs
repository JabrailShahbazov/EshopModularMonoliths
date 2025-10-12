namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductRequest(ProductDto Product);
public record UpdateRecordResponse(bool IsSuccess);

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/products", async (UpdateProductRequest request, ISender sender) =>
        {
            var product  = request.Adapt<UpdateProductCommand>();
            
            var result = await sender.Send(product);
            
            var response = result.Adapt<UpdateRecordResponse>();
            
            return Results.Ok(response);
        }).WithName("UpdateProduct")
          .Produces<UpdateRecordResponse>()
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .ProducesProblem(StatusCodes.Status404NotFound)
          .WithSummary("Updates an existing product")
          .WithDescription("Updates an existing product with the provided details.");
    }
}