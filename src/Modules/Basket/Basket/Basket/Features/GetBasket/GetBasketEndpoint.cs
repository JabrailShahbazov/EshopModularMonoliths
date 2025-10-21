namespace Basket.Basket.Features.GetBasket;

public record GetBasketRequest(string UserName);

public record GetBasketResponse(ShoppingCartDto ShoppingCart);

public class GetBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/basket/{userName}", async (string username, ISender sender) =>
        {
            var request = await sender.Send(new GetBasketQuery(username));

            var response = request.Adapt<GetBasketResponse>();
            
            return Results.Ok(response);
        })        .WithName("GetBasket")
        .Produces<GetBasketResponse>()
        .Produces(StatusCodes.Status400BadRequest)
        .WithSummary("Get Basket")
        .WithDescription("Get the shopping basket for a specific user by username.");
    }
}