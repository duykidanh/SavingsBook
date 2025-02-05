namespace SavingsBook.Application.Contracts.Common;

public class PageResultDto<T>
{
    public long TotalCount { get; set; }
    public IReadOnlyList<T>? Items { get; set; }
}