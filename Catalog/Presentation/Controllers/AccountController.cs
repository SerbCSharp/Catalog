using Catalog.Infrastructure.Repositories.Identity;
using Catalog.Infrastructure.Repositories.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Catalog.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly JWTConfiguration _options;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> user, SignInManager<IdentityUser> signIn, IOptions<JWTConfiguration> optAccess)
        {
            _options = optAccess.Value;
            _userManager = user;
            _signInManager = signIn;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(OtherParamUser paramUser)
        {
            var user = new IdentityUser { UserName = paramUser.UserName, Email = paramUser.Email };
            var result = await _userManager.CreateAsync(user, paramUser.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                List<Claim> claims = new List<Claim>
                {
                    new Claim("Profession", paramUser.Profession.ToString()),
                    new Claim("SeniorManager", paramUser.SeniorManager.ToString()),
                    new Claim(ClaimTypes.Email, paramUser.Email)
                };
                await _userManager.AddClaimsAsync(user, claims);
            }
            else
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(ParamUser paramUser)
        {
            var user = await _userManager.FindByEmailAsync(paramUser.Email);
            var result = await _signInManager.PasswordSignInAsync(user, paramUser.Password, false, false);
            if (result.Succeeded)
            {
                var claims = await _userManager.GetClaimsAsync(user);
                var token = GetToken(user, claims);
                return Ok(token);
            }
            return BadRequest();
        }

        private string GetToken(IdentityUser user, IEnumerable<Claim> prinicpal)
        {
            var claims = prinicpal.ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

            var jwt = new JwtSecurityToken(
                    issuer: _options.Issuer,
                    audience: _options.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
