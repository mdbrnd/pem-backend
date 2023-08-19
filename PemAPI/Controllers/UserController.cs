using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PemAPI.Models;
using PemAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PemAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [EnableCors("AllowMyOrigin")]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // GET: api/users
        //[HttpGet]
        //[Authorize]
        //public async Task<ActionResult<IEnumerable<<User>>> Get()
        //{
        //    // You might want to restrict this method to admin users or apply some other check
        //    // because returning all users might pose a privacy concern.

        //    var userId = GetUserIdFromToken();
        //    if (userId == null)
        //    {
        //        return BadRequest("Invalid token.");
        //    }

        //    var users = await _userService.GetAllUsersAsync();
        //    return Ok(users);
        //}


        // GET api/users/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Get(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            // Depending on your security model, you might want to ensure that the user requesting
            // the data has the rights to access information about the user with the given ID.

            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(new UserDTO { Id = user.Id, Username = user.Username, Email = user.Email });
        }


        // POST api/users
        [HttpPost]
        public async Task<ActionResult<UserDTO>> Post([FromBody] CreateUserModel newUserModel)
        {
            if (newUserModel.Email.IsNullOrEmpty() || newUserModel.Password.IsNullOrEmpty() || newUserModel.Username.IsNullOrEmpty())
            {
                return BadRequest("Empty parameters");
            }
            // Check if user already exists
            var userExists = await _userService.GetUserAsync(newUserModel.Email) == null ? false : true;
            if (userExists)
            {
                return BadRequest("User already exists");
            }

            // Authorization checks, for example ensuring only admins can create new users

            string salt = HashingService.GenerateSalt();
            string hashedPassword = HashingService.HashPassword(newUserModel.Password, salt);

            var newUser = await _userService.CreateUserAsync(new User { Username = newUserModel.Username, Email = newUserModel.Email, PasswordHash = hashedPassword, Salt = salt });
            if (newUser == null)
            {
                return BadRequest("Failed to create the user.");
            }

            return Ok(new UserDTO { Id = newUser.Id, Username = newUser.Username, Email = newUser.Email });
        }


        // PUT api/users/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Put(int id, [FromBody] CreateUserModel user)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            if (user.Email.IsNullOrEmpty() || user.Password.IsNullOrEmpty() || user.Username.IsNullOrEmpty())
            {
                return BadRequest("Empty parameters");
            }

            // Fetch the user to update
            var userToUpdate = await _userService.GetUserAsync(id);
            if (userToUpdate == null)
            {
                return NotFound("User does not exist.");
            }

            // Update and save the user using a service
            var updatedUser = await _userService.UpdateUserAsync(new User { Username = user.Username, Email = user.Email, PasswordHash = user.Password });
            if (updatedUser == null)
            {
                return BadRequest("Failed to update the user.");
            }

            return Ok(new UserDTO { Id = updatedUser.Id, Username = updatedUser.Username, Email = updatedUser.Email });
        }


        // DELETE api/users/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            // TODO: validate admin
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            // Ensure the user to delete exists
            var userToDelete = await _userService.GetUserAsync(id);
            if (userToDelete == null)
            {
                return NotFound("User does not exist.");
            }

            // Delete the user using a service
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return BadRequest(false);
            }

            return Ok(true);
        }

    }
}
