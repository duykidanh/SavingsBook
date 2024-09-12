namespace SavingsBook.Application.Contracts.Common;

public abstract class AuditedEntityDto<T> 
{
    public DateTime CreationTime { get; set; }
    public T? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public T? LastModifierId { get; set; }
    public T? Id { get; set; }
}