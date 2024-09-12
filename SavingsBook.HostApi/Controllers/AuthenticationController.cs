using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SavingsBook.Application.Contracts.Authentication;
using SavingsBook.Infrastructure.Authentication;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace SavingsBook.HostApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IConfiguration _configuration;

        // GET: api/Authentication
        public AuthenticationController(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }


        // POST: api/Authentication
        [HttpPost("/api/auth/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto input)
        {
            var user = await _userManager.FindByEmailAsync(input.Email);
            if (user is not null)
            {
                return StatusCode(400, new { message = "Email already in use" });
            }

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

            var addUserToRoleResult = await _userManager.AddToRoleAsync(user, "Admin");
            if (!createUserResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return StatusCode(400,
                    new { message = $"Add role to user failed, {addUserToRoleResult.Errors?.First()?.Description}" });
            }

            return StatusCode(200, new { message = "Create account successfully" });
        }

        [HttpPost("/api/auth/login")]
        public async Task<AuthenticationResponseDto> Login([FromBody] LoginDto input)
        {
            var user = await _userManager.FindByNameAsync(input.Username);
            if (user is null)
                return new AuthenticationResponseDto { Message = "Invalid email/password", Success = false };

            if (!await _userManager.CheckPasswordAsync(user, input.Password))
            {
                return new AuthenticationResponseDto { Message = "Password is incorrect", Success = false };
            }

            await _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(user, input.Password, false, true);

            return await GetJwtTokenAsync(user);
        }

        #region Private method

        private async Task<AuthenticationResponseDto> GetJwtTokenAsync(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var roles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(n => new Claim(ClaimTypes.Role, n)));
            var jwtToken = GetToken(authClaims);

            return new AuthenticationResponseDto
            {
                Message = "Login successfully",
                Success = true,
                AccessToken = new AuthenticationResponseDto.TokenType()
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtToken), ExpiresTime = jwtToken.ValidTo
                }
            };
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(7),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        #endregion


        // PUT: api/Authentication/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }
        //
        // // DELETE: api/Authentication/5
        // [HttpDelete("{id}")]
        // public void Delete(int id)
        // {
        // }
    }
}