namespace ORBIT9000.Core.Abstractions.Data.Entities
{
    public abstract class ExtendedEntity<TIdentityType> : IExtendedEntity<TIdentityType>
        where TIdentityType : struct
    {
        public TIdentityType CreatedBy { get; } = default;
        public DateTime CreatedOn { get; } = default;
        public TIdentityType DeletedBy { get; } = default;
        public DateTime? DeletedOn { get; } = default;
        public TIdentityType Id { get; init; } = default;
        public DateTime? ModifiedOn { get; } = default;
        public TIdentityType ModifiedBy { get; } = default;

        object? IEntity.Id
        {
            get => this.Id;
            set => ((IEntity)this).Id = value;
        }
    }
}