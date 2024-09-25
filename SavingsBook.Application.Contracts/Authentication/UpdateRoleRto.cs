using System.ComponentModel.DataAnnotations;

namespace SavingsBook.Application.Contracts.Authentication;

public class UpdateRoleRto
{
    [Required]
    public string[] Roles { get; set; }
}