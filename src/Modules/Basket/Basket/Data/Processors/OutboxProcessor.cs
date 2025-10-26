using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Basket.Data.Processors;

public class OutboxProcessor(IServiceProvider serviceProvider, IBus bus, ILogger<OutboxProcessor> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();

                var dbContext = scope.ServiceProvider.GetRequiredService<BasketDbContext>();

                var outboxMassages = await dbContext.OutboxMessages
                    .Where(x => x.ProcessedOn == null)
                    .ToListAsync(stoppingToken);

                foreach (var message in outboxMassages)
                {
                    var eventType = Type.GetType(message.Type);

                    if (eventType == null)
                    {
                        logger.LogWarning($"Outbox message type {message.Type} could not be found");
                        continue;
                    }
                    
                    var eventMessage = JsonSerializer.Deserialize(message.Content, eventType);

                    if (eventMessage == null)
                    {
                        logger.LogWarning($"Outbox message content could not be deserialized to type {message.Type}");
                        continue;
                    }
                    
                    await bus.Publish(eventMessage, stoppingToken);
                    
                    message.ProcessedOn = DateTime.UtcNow;
                    
                    logger.LogInformation($"Successfully processed outbox message ID: {message.Id} of type {message.Type}");
                }
                
                await dbContext.SaveChangesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing outbox messages");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}