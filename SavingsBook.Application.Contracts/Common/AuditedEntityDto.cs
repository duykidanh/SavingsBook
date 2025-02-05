using SavingsBook.Domain.Common;

namespace SavingsBook.Application.Contracts.Common;

public abstract class AuditedEntityDto<T> : AggregateRoot<T>
{
    public DateTime CreationTime { get; set; }
    public T? CreatorId { get; set; }
    public DateTime? LastModificationTime { get; set; }
    public bool IsActive { get; set; }
    public T? LastModifierId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletionTime { get; set; }
    public T? DeleterId { get; set; }
    public string Description { get; set; }
    public string Keyword { get; set; }
}