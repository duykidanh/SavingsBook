using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavingsBook.Infrastructure.Authentication;

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
    [HttpGet("api/getroles")]
    public async Task<IActionResult> GetRoles()
    {
        var roles = _roleManager.Roles.ToList();
        await Task.FromResult(roles);
        if (roles == null)
        {
            return StatusCode(400, new { message = "Cannot get list roles" });
        }

        return StatusCode(200, roles);
    }

    [HttpPost("/api/role/create")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            return StatusCode(400, new { message = "Role already exists" });
        }

        role = new ApplicationRole { Name = roleName, NormalizedName = roleName.ToUpper() };
        var addToRolesList = await _roleManager.CreateAsync(role);
        if (!addToRolesList.Succeeded)
        {
            return StatusCode(400, new {message = "Cannot create role" });
        }
        return StatusCode(200, new {message = "Create role Successfully" });
    }

    [HttpPost("/api/role/update")]
    public async Task<IActionResult> UpdateRole(string id,string newName)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return StatusCode(400, new { message = "Role does not exist" });
        }
        role.Name = newName;
        role.NormalizedName = newName.ToUpper();
        var updateRoleName = await _roleManager.UpdateAsync(role);
        if (!updateRoleName.Succeeded)
        {
            return StatusCode(400, new {message = "Cannot update role" });
        }
        return StatusCode(200, new {message = "Update role Successfully" });
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