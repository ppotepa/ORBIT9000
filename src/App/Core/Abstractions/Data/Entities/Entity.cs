namespace ORBIT9000.Core.Abstractions.Data.Entities
{
    public abstract class Entity<TIdentityType> : IEntity<TIdentityType>
        where TIdentityType : struct
    {
        public TIdentityType Id { get; init; } = default;
        dynamic IEntity.Id { get; set; }
    }
}
