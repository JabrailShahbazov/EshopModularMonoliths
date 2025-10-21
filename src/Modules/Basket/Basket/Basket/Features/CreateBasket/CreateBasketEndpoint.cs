namespace Basket.Basket.Features.CreateBasket;

public record CreateBasketRequest(ShoppingCartDto ShoppingCart);

public record CreateBasketResponse(Guid Id);

public class CreateBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket", async (CreateBasketRequest request, ISender sender) =>
            {
                var command = request.Adapt<CreateBasketCommand>();

                var result = await sender.Send(command);

                var response = result.Adapt<CreateBasketResponse>();

                return Results.Created($"/basket/{response.Id}", response);
            }).Produces<CreateBasketResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Creates a new shopping basket")
            .WithDescription("Creates a new shopping basket for a user with the provided items.");
    }
}