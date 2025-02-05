using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using SavingsBook.Application.Contracts.Authentication;

namespace SavingsBook.Application.RolesConfig;

public class RoleSeeder
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleSeeder(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task SeedRoles()
    {
        var roles = new List<string> { "Admin", "User"};
        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                // Tạo role mới
                role = new ApplicationRole
                {
                    Id = ObjectId.GenerateNewId(), // Khởi tạo ObjectId
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(), // Tạo ConcurrencyStamp ngẫu nhiên
                    IsDeleted = false, // Mặc định là false
                    IsActive = true // Kích hoạt
                };

                var result = await _roleManager.CreateAsync(role);
            }

        }

        Console.WriteLine("Roles seeded successfully");
    }
}