using Basket.Basket.Features.UpdateItemPriceInBasket;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Messaging.Events;

namespace Basket.Basket.EventHandlers;

public class ProductPriceChangedIntegrationEventHandler(
    ISender sender,
    ILogger<ProductPriceChangedIntegrationEventHandler> logger)
    : IConsumer<ProductPriceChangeIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ProductPriceChangeIntegrationEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        var result = await sender.Send(new UpdateItemPriceInBasketCommand(context.Message.ProductId, context.Message.Price));

        if (!result.IsSuccess)
        {
            logger.LogError("Error updating item price in basket for ProductId: {ProductId}", context.Message.ProductId);
        }

        logger.LogInformation("Price for product {ProductId} updated to {Price} in all shopping carts.", context.Message.ProductId,
            context.Message.Price);
    }
}