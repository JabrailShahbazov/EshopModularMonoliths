namespace Shared.DDD.Domain.Entities;

public interface ICreationEntity
{
    public DateTime? CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}