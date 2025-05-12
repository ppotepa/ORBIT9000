namespace ORBIT9000.Core.Abstractions.Data.Entities
{
    public interface IExtendedEntity<TIdentityType> : IEntity<TIdentityType>
        where TIdentityType : struct
    {
        TIdentityType CreatedBy { get; }
        DateTime CreatedOn { get; }
        TIdentityType DeletedBy { get; }
        DateTime? DeletedOn { get; }
        TIdentityType ModifiedBy { get; }
        DateTime? ModifiedOn { get; }
    }

    public interface IExtendedEntity
    {
        object CreatedBy { get; }
        object CreatedOn { get; }
        object DeletedBy { get; }
        object DeletedOn { get; }
        object ModifiedBy { get; }
        object ModifiedOn { get; }
    }
}
