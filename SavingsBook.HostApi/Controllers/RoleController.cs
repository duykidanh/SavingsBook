using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SavingsBook.Application.Contracts.Authentication;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]

public class RoleController: ControllerBase
{
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleController(RoleManager<ApplicationRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {

        var roles = _roleManager.Roles.ToList();


        if (roles == null || !roles.Any())
        {
            return StatusCode(400, new { message = "Cannot get list roles" });
        }

        return StatusCode(200, new { roles });
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            return StatusCode(400, new { message = "Role already exists" });
        }

        role = new ApplicationRole
        {
            Id = ObjectId.GenerateNewId(),
            Name = roleName,
            NormalizedName = roleName.ToUpper(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            IsDeleted = false,
            IsActive = true
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return StatusCode(400, new { message = "Cannot create role", errors = result.Errors });
        }

        return StatusCode(200, new { message = "Role created successfully", role });
    }

    [HttpPost("/api/role/update")]
    public async Task<IActionResult> UpdateRole(string id, string newName, ObjectId currentUserId)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return StatusCode(400, new { message = "Role does not exist" });
        }

        role.Name = newName;
        role.NormalizedName = newName.ToUpper();
        role.LastModifierId = currentUserId;
        role.LastModificationTime = DateTime.UtcNow;

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            return StatusCode(400, new { message = "Cannot update role", errors = result.Errors });
        }

        return StatusCode(200, new { message = "Update role successfully" });
    }

    [HttpDelete("/api/role/delete")]
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return StatusCode(400, new { message = "Role does not exist" });
        }

        var deleteFromRolesList = await _roleManager.DeleteAsync(role);
        if (!deleteFromRolesList.Succeeded)
        {
            return StatusCode(400, new {message = "Cannot delete role" });
        }
        return StatusCode(200, new {message = "Delete role Successfully" });
    }
}