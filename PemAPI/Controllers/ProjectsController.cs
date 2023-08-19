using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PemAPI.Models;
using PemAPI.Services;
using System.Security.Claims;

namespace PemAPI.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [EnableCors("AllowMyOrigin")]
    public class ProjectsController : BaseController
    {
        private readonly ProjectsService _projectsService;

        public ProjectsController(ProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        // GET: api/projects
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Project>>> Get()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var projects = await _projectsService.GetProjectsAsync((int)userId);
            return Ok(projects);
        }

        // GET api/projects/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Project>> Get(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var project = await _projectsService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound("Project does not exist");
            }

            if (!project.MemberUserIds.Contains((int)userId) && project.OwnerUserId != (int)userId)
            {
                return Forbid("User is not a member of this project.");
            }

            return Ok(project);
        }

        // POST api/projects
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Project>> Post([FromBody] CreateProjectModel project)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var createdProject = await _projectsService.CreateProjectAsync(new Project(ownerUserId: (int)userId, name: project.Name, description: project.Description));
            return Ok(createdProject);
        }


        // PUT api/projects/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Project>> Put(int id, [FromBody] Project project)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            // Check if the user has permission to update the project.
            var existingProject = await _projectsService.GetProjectAsync(id);
            if (existingProject == null)
            {
                return NotFound("Project does not exist");
            }

            if (existingProject.OwnerUserId != userId)
            {
                return Forbid("You don't have permission to update this project.");
            }

            var updatedProject = await _projectsService.UpdateProjectAsync(project);

            if (updatedProject == null)
            {
                return BadRequest("Failed to update the project.");
            }

            return Ok(updatedProject);
        }


        // DELETE api/projects/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var project = await _projectsService.GetProjectAsync(id);
            if (project == null)
            {
                return NotFound("Project does not exist");
            }

            if (!project.MemberUserIds.Contains((int)userId) && project.OwnerUserId != (int)userId)
            {
                return Forbid("User is not a member of this project.");
            }

            var success = await _projectsService.DeleteProjectAsync(id);
            if (!success)
            {
                return BadRequest(false);
            }
            return Ok(success);
        }
    }
}
