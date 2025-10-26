namespace Basket.Basket.Modules;

public class OutboxMessage : Entity<Guid>
{
    public string Type { get; set; } = default!;
    
    public string Content { get; set; } = default!;

    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    public DateTime? ProcessedOn { get; set; } = default!;
}