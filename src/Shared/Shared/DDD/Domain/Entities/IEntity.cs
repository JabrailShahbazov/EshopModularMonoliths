namespace Shared.DDD.Domain.Entities;

public interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

public interface IEntity : ICreationEntity, IEditEntity
{
}