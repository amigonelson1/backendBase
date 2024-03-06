using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DataAccess;
using Business.Interfaces;
using Business.DTOs;
using AutoMapper;

namespace APIMAPLED.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        ///<summary>Login User</summary>
        ///<param name="model">Schema JSON from body</param>
        ///<returns></returns>
        //POST : /api/Account/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponseDTO>> Login([FromBody] UserCredentialsDTO model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        return Ok(await _tokenService.TokenBuilder(model));
                    }
                    else
                    {
                        return BadRequest("Incorrect password");
                    }
                }
                else
                {
                    return BadRequest("The user is not register in the app");
                }
            }
            catch (Exception exc)
            {
                return BadRequest(new { message = exc.Message });
            }
        }

        ///<summary>Create new user in the database</summary>
        ///<param name="model">Schema JSON from body</param>
        ///<returns></returns>
        //POST : /api/Account/Register
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterModelDTO model)
        {
            var existUserName = await _userManager.FindByNameAsync(model.UserName);
            if (existUserName != null)
            {
                return BadRequest("The UserName already exists in the datebase");
            }

            var existEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existEmail != null)
            {
                return BadRequest("The Email already exists in the datebase");
            }

            // User user = _mapper.Map<User>(model);

            User user = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user: user, password: model.Password);
                if (result.Succeeded)
                {
                    var userDB = await _userManager.FindByNameAsync(user.UserName) ?? throw new ArgumentException("The user in not in the database");
                    return CreatedAtRoute("GetUser", new { UserName = userDB.UserName }, _mapper.Map<UserDTO>(userDB));
                }
                return BadRequest($"Error: {result.Errors}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        ///<summary>Get User</summary>
        ///<param name="UserName">UserName of user</param>
        ///<returns></returns>
        //POST : /api/Account/GetUser/cleon
        [HttpGet("GetUser/{UserName}", Name = "GetUser")]
        public async Task<ActionResult<UserDTO>> GetUser([FromRoute] string UserName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (user != null)
                {
                    return Ok(_mapper.Map<UserDTO>(user));
                }
                return NotFound("The user in not in the database");
            }
            catch (Exception exc)
            {
                return BadRequest(new { message = exc.Message });
            }
        }
    }
}