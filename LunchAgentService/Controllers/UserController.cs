//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using LunchAgentService.Entities;
//using LunchAgentService.Helpers;
//using LunchAgentService.Services.UserService;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

//namespace LunchAgentService.Controllers
//{
//    [Route("api/user")]
//    [Authorize]
//    public class UserController : ControllerBase
//    {
//        private readonly IUserService _userService;
//        private readonly AppSettings _appSettings;

//        public UserController(IUserService userService, IOptions<AppSettings> appSettings)
//        {
//            _userService = userService;
//            _appSettings = appSettings.Value;
//        }

//        [HttpPost("login")]
//        [AllowAnonymous]
//        public IActionResult Authenticate([FromBody]UserApi userApi)
//        {
//            var user = _userService.Authenticate(userApi.Username, userApi.Password);

//            if (user == null)
//                return BadRequest(new { message = "Username or password is incorrect" });

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new[]
//                {
//                    new Claim(ClaimTypes.Name, user.Username),
//                    new Claim(ClaimTypes.Role, user.Role)
//                }),
//                Expires = DateTime.UtcNow.AddDays(7),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };
//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            var tokenString = tokenHandler.WriteToken(token);

//            return Ok(new
//            {
//                user.Username,
//                Token = tokenString
//            });
//        }

//        [HttpPost("register")]
//        [AllowAnonymous]
//        public IActionResult Register([FromBody]UserApi userApi)
//        {
//            try
//            {
//                _userService.Create(userApi);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public IActionResult GetAll()
//        {
//            var users = _userService.GetAll();
//            return Ok(users);
//        }

//        [HttpGet("{id}")]
//        public IActionResult GetById(string id)
//        {
//            var user = _userService.GetById(id);

//            if (user == null)
//            {
//                return NotFound();
//            }

//            return Ok(user);
//        }

//        [HttpPut("{id}")]
//        public IActionResult Update(int id, [FromBody]UserApi userApi)
//        {
//            var currentUserId = int.Parse(User.Identity.Name);
//            if (id != currentUserId && User.IsInRole(Role.SuperAdmin) == false)
//            {
//                return Forbid();
//            }

//            try
//            {
//                // save 
//                _userService.Update(userApi);
//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return BadRequest(new { message = ex.Message });
//            }
//        }

//        [HttpDelete("{id}")]
//        public IActionResult Delete(string id)
//        {
//            var currentUserId = User.Identity.Name;
//            if (id != currentUserId && User.IsInRole(Role.SuperAdmin) == false)
//            {
//                return Forbid();
//            }

//            _userService.Delete(id);
//            return Ok();
//        }
//    }
//}