namespace ORBIT9000.Abstractions.Data.Entities
{
    public interface IExtendedEntity<TIdentityType> : IEntity<TIdentityType>
        where TIdentityType : struct
    {
        TIdentityType CreatedBy { get; internal set; }
        DateTime CreatedOn { get; internal set; }
        TIdentityType DeletedBy { get; internal set; }
        DateTime? DeletedOn { get; internal set; }
        TIdentityType ModifiedBy { get; internal set; }
        DateTime? ModifiedOn { get; internal set; }
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
