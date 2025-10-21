namespace Basket.Basket.Features.RemoveItemFromBasket;

public record RemoveItemFromBasketResponse(Guid Id);

public class RemoveItemFromBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}/items/{productId}", async ([FromRoute] string userName,
                [FromRoute] Guid productId,
                ISender sender) =>
            {
                var command = new RemoveItemFromBasketCommand(userName, productId);

                var result = await sender.Send(command);

                var response = result.Adapt<RemoveItemFromBasketResponse>();

                return Results.Ok(response);
            }).Produces<RemoveItemFromBasketResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove item from user's basket")
            .WithDescription("Removes an item from the specified user's shopping basket.");
    }
}