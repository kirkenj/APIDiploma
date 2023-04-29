using Logic.Interfaces;
using Logic.Exceptions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.RequestModels.Authorize;
using AutoMapper;
using Database.Entities;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;

        public AuthorizeController(IAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login(string login, string password)
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Logout();
            }

            var result = await _accountService.GetUser(login, password);
            if (result is null)
            {
                return BadRequest("Invalid login or password");
            }

            var claims = new List<Claim>
            {
                new Claim(Constants.IncludeModels.UserIdentitiesTools.NameKey, result.Login),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.IDKey, result.ID.ToString()),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.RoleKey, (await _accountService.GetRole(result.RoleId) ?? throw new ObjectNotFoundException($"Role not found by id = {result.RoleId}")).Name)
            };
    

            ClaimsIdentity claimsIdentity = new(claims, "Cookies");
            await Request.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return Ok();
        }

        [HttpPost(nameof(Logout))]
        public ActionResult Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Request.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserModel registerModel)
        {
            var (succed, explanation) = await _accountService.AddUser(_mapper.Map<User>(registerModel));
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
