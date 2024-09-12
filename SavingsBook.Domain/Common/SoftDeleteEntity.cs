namespace SavingsBook.Domain.Common
{
	public interface  ISoftDeleteEntity
	{
		public bool IsDeleted { get; set; }
		public DateTime? DeletionTime { get; set; }
		public Guid? DeleterId { get; set; }

		public void SoftDelete(Guid? deleterId)
		{
			IsDeleted = true;
			DeletionTime = DateTime.UtcNow;
			DeleterId = deleterId;
		}

		public void UndoDelete()
		{
			IsDeleted = false;
			DeletionTime = null;
			DeleterId = null;
		}
	}
}
