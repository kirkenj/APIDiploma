using Logic.Interfaces;
using Logic.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Web.RequestModels.Authorize;
using AutoMapper;
using Database.Entities;
using System.IdentityModel.Tokens.Jwt;
using Web.Models.JWTModels;
using Microsoft.IdentityModel.Tokens;
using Web.Models.Authorize;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;
        private readonly IHashProvider _hashProvider;
        private readonly JWTSettings _jwtSettings;

        public AuthorizeController(IAccountService accountService, IRoleService roleService, IHashProvider hashProvider, IMapper mapper, JWTSettings jwtSettings)
        {
            _accountService = accountService;
            _roleService = roleService;
            _hashProvider = hashProvider;
            _mapper = mapper;
            _jwtSettings = jwtSettings;
        }

        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody]LoginModel loginModel)
        {
            if (User.Identity?.IsAuthenticated ?? false) Logout();
            loginModel.Password = _hashProvider.GetHash(loginModel.Password);
            var result = await _accountService.FirstOrDefaultAsync(u => u.Login == loginModel.Login && u.PasswordHash == loginModel.Password);
            if (result is null) return BadRequest("Invalid login or password");
            
            var claims = new List<Claim>
            {
                new Claim(Constants.IncludeModels.UserIdentitiesTools.NameKey, result.Login),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.IDKey, result.ID.ToString()),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.RoleKey, (await _roleService.FirstOrDefaultAsync(r => r.ID == result.RoleId) ?? throw new ObjectNotFoundException($"Role not found by id = {result.RoleId}")).Name),
                new Claim(Constants.IncludeModels.UserIdentitiesTools.IsConfirmedKey, result.IsConfirmed.ToString())
            };

            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var jwt = new JwtSecurityToken(
                    issuer: _jwtSettings.Issuer,
                    audience: _jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromDays(3)),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                );

            return Ok(new JwtSecurityTokenHandler().WriteToken(jwt));
        }

        [HttpPost(nameof(Logout))]
        public ActionResult Logout()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                Request.HttpContext.SignOutAsync();
                return Ok();
            }

            return BadRequest();
        }

        [HttpPost(nameof(Register))]
        public async Task<ActionResult> Register(RegisterUserModel registerModel)
        {
            await _accountService.AddAsync(_mapper.Map<User>(registerModel));
            return Ok();
        }
    }
}
