namespace SavingsBook.Application.Contracts.Common;

public interface IAuditedEntityDto<T> 
{
    DateTime CreationTime { get; set; }
    T? CreatorId { get; set; }
    DateTime? LastModificationTime { get; set; }
    bool IsActive { get; set; }
    T? LastModifierId { get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletionTime { get; set; }
    T? DeleterId { get; set; }
    T? Id { get; set; }
    string Description { get; set; }
    string Keyword { get; set; }
}