namespace SavingsBook.Domain.Common
{
	public abstract class AuditedAggregateRoot<T> : AggregateRoot<T> ,ISoftDeleteEntity
	{
		public DateTime CreationTime { get; set; }

		public T? CreatorId { get; set; }
		public DateTime? LastModificationTime { get; set; }

		public T? LastModifierId { get; set; }

		public bool IsDeleted { get; set; }
		public DateTime? DeletionTime { get; set; }
		public T? DeleterId { get; set; }
        Guid? ISoftDeleteEntity.DeleterId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

}
