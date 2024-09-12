namespace SavingsBook.Domain.Common
{
	public abstract class AggregateRoot<T> 
	{
		public T Id { get; set; }
		public string?  ConcurrencyStamp { get; set; }
	}
}
