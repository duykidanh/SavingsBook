using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SavingsBook.Infrastructure.Authentication;

namespace SavingsBook.HostApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    [HttpGet]
    public async  Task<IActionResult> GetUserInfo(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }
        
        throw new NotImplementedException();
    }

   
}