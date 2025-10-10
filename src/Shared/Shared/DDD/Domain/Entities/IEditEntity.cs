namespace Shared.DDD.Domain.Entities;

public interface IEditEntity
{
    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}