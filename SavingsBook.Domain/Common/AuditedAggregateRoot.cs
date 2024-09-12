namespace SavingsBook.Domain.Common
{
	public abstract class AuditedAggregateRoot<T> : AggregateRoot<Guid> ,ISoftDeleteEntity
	{
		public DateTime CreationTime { get; set; }

		public Guid? CreatorId { get; set; }
		public DateTime? LastModificationTime { get; set; }

		public Guid? LastModifierId { get; set; }

		public bool IsDeleted { get; set; }
		public DateTime? DeletionTime { get; set; }
		public Guid? DeleterId { get; set; }
	}
}
