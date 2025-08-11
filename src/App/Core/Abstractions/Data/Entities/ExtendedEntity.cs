namespace ORBIT9000.Abstractions.Data.Entities
{
    public abstract class ExtendedEntity<TIdentityType> : IExtendedEntity<TIdentityType>
        where TIdentityType : struct
    {
        public TIdentityType CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public TIdentityType DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public TIdentityType Id { get; init; }
        public DateTime? ModifiedOn { get; set; }
        public TIdentityType ModifiedBy { get; set; }

        object? IEntity.Id
        {
            get => Id;
            set => ((IEntity)this).Id = value;
        }
    }
}