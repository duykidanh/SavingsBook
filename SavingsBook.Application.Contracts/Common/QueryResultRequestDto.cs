using System.ComponentModel.DataAnnotations;

namespace SavingsBook.Application.Contracts.Common;

public class QueryResultRequestDto
{
    [Range(0, int.MaxValue)]
    public int SkipCount { get; set; }
    [Range(0, int.MaxValue)]
    public int MaxResultCount { get; set; }
    public string? Sorting { get; set; }
    public string? Filter { get; set; }
    public bool IsDeleted { get; set; } = false;
}