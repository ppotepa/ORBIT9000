namespace ORBIT9000.Core.Abstractions.Data.Entities
{
    public abstract partial class Entity<TIdentityType> : IEntity<TIdentityType>
        where TIdentityType : struct
    {
        public TIdentityType Id { get; init; } = default;
        dynamic IEntity.Id { get; set; }
    }

    public interface IEntity
    {
        object Id { get; set; }

        object this[string index]
        {
            get => this.GetType().GetProperty(index)?.GetValue(this)!;
        }
    }

    public interface IEntity<TIdentityType> : IEntity
        where TIdentityType : struct
    {
        new TIdentityType Id
        {
            get
            {
                return (TIdentityType)this.GetType().GetProperty("Id")?.GetValue(this)!;
            }

            set => this.GetType().GetProperty("Id")?.SetValue(this, value);
        }
    }
}
