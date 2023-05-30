using AutoMapper;
using Database.Entities;
using Logic.Exceptions;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebFront.Models.Authorize;

namespace WebFront.Controllers
{
    [AllowAnonymous]
    public class AuthorizeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IHashProvider _hashProvider;

        public AuthorizeController(IAccountService accountService, IRoleService roleService, IHashProvider hashProvider, IMapper mapper)
        {
            _accountService = accountService;
            _roleService = roleService;
            _hashProvider = hashProvider;
            _mapper = mapper;
        }

        [HttpGet("Login")]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (User.Identity?.IsAuthenticated ?? false) await Logout();
            loginModel.Password = _hashProvider.GetHash(loginModel.Password);
            var result = await _accountService.FirstOrDefaultAsync(u => u.Login == loginModel.Login && u.PasswordHash == loginModel.Password);
            if (result is null) return BadRequest("Invalid login or password");

            var role = await _roleService.FirstOrDefaultAsync(r => r.ID == result.RoleId) ?? throw new ObjectNotFoundException($"Role not found by id = {result.RoleId}");

            var claims = new List<Claim>
            {
                new Claim(Constants.IncludeModels.UserIdentitiesTools.NameKey, result.Login),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.IDKey, result.ID.ToString()),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.RoleKey, role.Name),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.IsAdminKey, _accountService.IsAdmin(result).ToString())
            };

            ClaimsIdentity claimsIdentity = new(claims, "Cookies");
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return RedirectToAction("Index", "Home");
        }



        [HttpPost(nameof(Logout))]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/login");
            }

            return BadRequest();
        }

        [HttpGet(nameof(Register))]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserModel registerModel)
        {
            if (registerModel.Password != registerModel.ConfirmPassword)
            {
                return BadRequest("Password is not equal to confirmPassword");
            }

            if (registerModel.Password == "password")
            {
                return BadRequest("Password can not be 'password'");
            }

            await _accountService.AddAsync(_mapper.Map<User>(registerModel));
            return Ok();
        }
    }
}
