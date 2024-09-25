using Microsoft.AspNetCore.Identity;
using SavingsBook.Infrastructure.Authentication;

namespace SavingsBook.Infrastructure.RolesConfig;

public class RoleSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleSeeder(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRoles()
    {
        var roles = new List<string> { "Admin", "User", "Staff" };
        foreach (var role in roles)
        {
            if (!(await _roleManager.RoleExistsAsync(role)))
            {
                await _roleManager.CreateAsync(new() { Name = role });
            }
        }

        Console.WriteLine("Roles seeded successfully");
    }
}