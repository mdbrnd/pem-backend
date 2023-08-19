using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using PemAPI.Models;
using PemAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PemAPI.Controllers
{
    [Route("api/projects")]
    [ApiController]
    public class ProjectMemberController : BaseController
    {
        private readonly UserService _userService;
        private readonly ProjectsService _projectsService;

        public ProjectMemberController(UserService userService, ProjectsService projectsService)
        {
            _userService = userService;
            _projectsService = projectsService;
        }

        // GET: api/projects/1/members
        [HttpGet("{projectId}/members")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> Get(int projectId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound("Project does not exist");
            }

            if (!project.MemberUserIds.Contains((int)userId) && project.OwnerUserId != (int)userId)
            {
                return Forbid("User is not a member of this project.");
            }

            IEnumerable<UserDTO> members = new List<UserDTO>();
            foreach (var userid in project.MemberUserIds)
            {
                var user = await _userService.GetUserAsync(userid);
                if (user != null)
                {
                    members = members.Append(new UserDTO { Id = user.Id, Username = user.Username, Email = user.Email});
                }
            }

            return Ok(members);
        }

        [HttpGet("{projectId}/members/{id}")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> Get(int projectId, int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound("Project does not exist.");
            }

            if (!project.MemberUserIds.Contains((int)userId) && project.OwnerUserId != (int)userId)
            {
                return Forbid("User is not a member of this project.");
            }

            var user = await _userService.GetUserAsync(id);
            if (user == null)
            {
                return NotFound("User does not exist.");
            }

            return Ok(new UserDTO { Id = user.Id, Username = user.Username, Email = user.Email });
        }


        [HttpPost("{projectId}/members")]
        [Authorize]
        public async Task<ActionResult<UserDTO>> AddMember(int projectId, [FromBody] int memberId)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound("Project does not exist.");
            }

            // Ensure the user trying to add members has appropriate permissions
            if (project.OwnerUserId != (int)userId)
            {
                return Forbid("Only the project owner can add members.");
            }

            if (project.MemberUserIds.Contains(memberId))
            {
                return BadRequest("User is already a member of this project.");
            }

            var userToAdd = await _userService.GetUserAsync(memberId);
            if (userToAdd == null)
            {
                return NotFound("User to be added does not exist.");
            }

            project.MemberUserIds.Add(memberId);
            await _projectsService.UpdateProjectAsync(project);

            return Ok(new UserDTO { Id = userToAdd.Id, Username = userToAdd.Username, Email = userToAdd.Email });
        }


        // DELETE api/projects/1/members/5
        [HttpDelete("{projectId}/members/{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> RemoveMember(int projectId, int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            if (userId == id)
            {
                return BadRequest("Cannot remove project owner.");
            }

            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound("Project does not exist.");
            }

            // Ensure the user trying to remove members has appropriate permissions. 
            if (project.OwnerUserId != (int)userId && id != (int)userId)
            {
                return Forbid("You do not have permission to remove members from this project.");
            }

            if (!project.MemberUserIds.Contains(id))
            {
                return NotFound("User is not a member of this project.");
            }

            var userRemoved = await _userService.GetUserAsync(id);
            if (userRemoved == null)
            {
                return NotFound("User to be removed does not exist.");
            }
            
            project.MemberUserIds.Remove(id);
            var success = await _projectsService.UpdateProjectAsync(project);
            if (success == null)
            {
                return BadRequest(false);
            }

            return Ok(true);
        }
    }
}
