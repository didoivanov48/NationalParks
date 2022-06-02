using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NationalParksAPI.Models;
using NationalParksAPI.Repository.IRepository;

namespace NationalParksAPI.Controllers
{
    [Authorize]
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository userRepository;

        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthenticationModel model)
        {
            var user = userRepository.Authenticate(model.Username, model.Password);
            if (user == null)
            {
                return BadRequest(new { message = "Username or Password is incorrect!" });
            }
            return Ok(user);
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthenticationModel model)
        {
            bool ifUsernameUnique = userRepository.IsUniqueUser(model.Username);
            if (!ifUsernameUnique)
            {
                return BadRequest(new { message = "Username already exists!" });
            }
            var user = userRepository.Register(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Error while registering!" });
            }

            return Ok();
        }
    }
}
