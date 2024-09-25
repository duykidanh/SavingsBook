using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavingsBook.Application.Contracts.Authentication;
using SavingsBook.Infrastructure.Authentication;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    public UserController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    [HttpGet("api/get-user-info")]
    public async  Task<IActionResult> GetUserInfo(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return StatusCode(400, new { message = "User not found." });
        }

        return StatusCode(200, user);
    }

    [HttpGet("api/get-users")]
    public async  Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.ToList();

        await Task.FromResult(users);
        if (users == null)
        {
            return StatusCode(400, new { message = "Users not found." });
        }

        var listUserRoles = new List<object>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            listUserRoles.Add(new { user = user, roles = roles });
        }
        return StatusCode(200, listUserRoles);
    }

    [HttpPost("api/add-user")]
    public async Task<IActionResult> AddUser([FromBody] RegisterDto input)
    {
        var user = await _userManager.FindByEmailAsync(input.Email);
        if (user is not null)
        {
            return StatusCode(400, new { message = "Email already in use" });
        }
        // user = _userManager.Users.SingleOrDefault(x => x.IdCardNumber == input.IdCardNumber);
        // await Task.FromResult(user);
        // if (user != null)
        // {
        //     return StatusCode(400, new { message = "ID Card Number already in use" });
        // }
        user = new ApplicationUser
        {
            Email = input.Email, UserName = input.Username, ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        var createUserResult = await _userManager.CreateAsync(user, input.Password);
        if (!createUserResult.Succeeded)
        {
            return StatusCode(400,
                new { message = $"Create user failed, {createUserResult.Errors?.First()?.Description}" });
        }

        var addUserToRoleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!createUserResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);
            return StatusCode(400,
                new { message = $"Add role to user failed, {addUserToRoleResult.Errors?.First()?.Description}" });
        }

        return StatusCode(200, new { message = "Create user successfully" });
    }

    [HttpPost("api/update-user")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateRto input)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return StatusCode(400, new { message = "User not found." });
        }

        user.Email = input.Email ?? user.Email;
        user.FullName = input.FullName ?? user.FullName;
        user.Address = input.Address ?? user.Address;
        user.IdCardNumber = input.IdCardNumber ?? user.IdCardNumber;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(400, new { message = "User update failed, {errors = result.Errors});" });
        }
        return StatusCode(200, new { message = "Update account successfully" });
    }

    [HttpPost("api/update-user-role")]
    public async Task<IActionResult> UpdateUserRole([FromQuery]string id, [FromBody] UpdateRoleRto roleNames)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return StatusCode(400, new { message = "User not found." });
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        var resetRole = await _userManager.RemoveFromRolesAsync(user, userRoles);
        if (!resetRole.Succeeded)
        {
            return StatusCode(400, new { message = "Reset role failed." });
        }

        foreach (var roleName in roleNames.Roles)
        {
            var updateRole = await _userManager.AddToRoleAsync(user, roleName);
            if (!updateRole.Succeeded)
            {
                return StatusCode(400, new { message = "Update role failed." });
            }
        }

        return StatusCode(200, new { message = "Update user's role successfully" });
    }

    [HttpDelete("api/delete-user")]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return StatusCode(400, new { message = "User not found." });
        }
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(400, new { message = "Cannot delete user." });
        }
        return StatusCode(200, new { message = "User successfully deleted." });
    }

    [HttpGet("api/get-user-roles")]
    public async  Task<IActionResult> GetUserRoles(string id)
    {
        var users = await _userManager.FindByIdAsync(id);

        if (users == null)
        {
            return StatusCode(400, new { message = "Users not found." });
        }
        var roles = await _userManager.GetRolesAsync(users);
        if (roles == null)
        {
            return StatusCode(400, new { message = "Roles not found." });
        }
        return StatusCode(200, roles);
    }
}