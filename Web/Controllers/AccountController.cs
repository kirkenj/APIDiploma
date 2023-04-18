using AutoMapper;
using Logic.Interfaces;
using Logic.RequestModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiplomaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService = null!; 
        private readonly IMapper _mapper;

        public AccountController(IAccountService appDBContext1, IMapper mapper)
        {
            _accountService = appDBContext1 ?? throw new ArgumentNullException(nameof(appDBContext1));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: api/<AccountController>
        [HttpGet]
        [Authorize("OnlyAdmin")]
        public async Task<IActionResult> GetAll()
        {
            return Ok( _mapper.Map<List<UserViewModel>>(await _accountService.GetUsers()));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var login = User.Identity?.Name;
            if (login == null)
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<UserViewModel>(await _accountService.GetUserByLoginAsync(login)));
        }

        [Authorize("OnlyAdmin")]
        public async Task<IActionResult> SetRole(int userId, int RoleID)
        {
            await _accountService.SetRoleAsync(userId, RoleID);
            return Ok();
        }
    }
}
