namespace Basket.Basket.Features.AddItemIntoBasket;

public record AddItemIntoBasketRequest(string Username, ShoppingCartItemDto ShoppingCartItemDto);

public record AddItemIntoBasketResponse(Guid Id);

public class AddItemIntoBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/basket/{username}/items", async ([FromRoute] string userName,
                                                       [FromBody] AddItemIntoBasketRequest request,
                                                       ISender sender) =>
            {
                var command = new AddItemIntoBasketCommand(userName, request.ShoppingCartItemDto);

                var result = await sender.Send(command);

                var response = result.Adapt<AddItemIntoBasketResponse>();

                return Results.Created($"basket/{response.Id}", response);
            }).Produces<AddItemIntoBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add item into user's basket")
            .WithDescription("Adds an item into the specified user's shopping basket.");
    }
}