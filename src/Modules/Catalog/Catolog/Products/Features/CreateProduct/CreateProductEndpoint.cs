namespace Catalog.Products.Features.CreateProduct;

public record CreateProductRequest(ProductDto Product);

public record CreateProductResponse(Guid Id);

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/products", async (CreateProductRequest request, IMediator mediator) =>
        {
            var command = request.Adapt<CreateProductCommand>();
            
            var result = await mediator.Send(command);
            
            var response  = result.Adapt<CreateProductResponse>();
            
            return Results.Created($"products/{response.Id}", response);
        }).WithName("CreateProduct")
          .Produces<CreateProductResponse>(StatusCodes.Status201Created)
          .ProducesProblem(StatusCodes.Status400BadRequest)
          .WithSummary("Creates a new product")
          .WithDescription("Creates a new product with the provided details.");
    }
}