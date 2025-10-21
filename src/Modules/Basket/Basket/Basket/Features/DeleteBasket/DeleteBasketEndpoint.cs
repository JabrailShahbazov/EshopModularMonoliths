namespace Basket.Basket.Features.DeleteBasket;

public record DeleteBasketResponse(bool IsSuccess);

public class DeleteBasketEndpoint :ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBasketCommand(userName));
           
            var response = result.Adapt<DeleteBasketResponse>();
            
            return Results.Ok(response);
        })
        .WithName("Delete Basket")
        .WithSummary("Deletes a basket for a given buyer userName.")
        .WithDescription("Deletes the shopping basket associated with the specified buyer userName from the system.")
        .Produces<DeleteBasketResponse>()
        .Produces(StatusCodes.Status404NotFound);
    }
}