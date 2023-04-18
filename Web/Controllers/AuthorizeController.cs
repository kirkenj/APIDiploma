using Logic.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Logic.RequestModels.Authorize;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DiplomaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AuthorizeController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/<AccountController>
        [HttpPost("Login")]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Logout();
            }

            var result = await _accountService.SignInAsync(login, password);
            if (result is null)
            {
                return BadRequest("Invalid login or password");
            }

            var claims = new List<Claim> 
            {
                new Claim(ClaimTypes.Name, result.Login),
                new Claim(ClaimTypes.Role, (await _accountService.GetRoleByIDAsync(result.RoleId)).Name)
            };

            ClaimsIdentity claimsIdentity = new(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }

        [HttpGet("Logout")]
        public ActionResult Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }

            return BadRequest();
        }

        // POST: AuthorizeControlle/Create
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserModel registerModel)
        {

            var q = registerModel.Validate(new System.ComponentModel.DataAnnotations.ValidationContext(User));
            if (q.Any())
            {
                return BadRequest(string.Join("\n", q));
            }

            var (succed, explanation) = await _accountService.AddUser(registerModel);
            if (succed)
            {
                return Ok();
            }
            else
            {
                return BadRequest(explanation);
            }
        }
    }
}
