using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PemAPI.Models;
using PemAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PemAPI.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [EnableCors("AllowMyOrigin")]
    public class IssuesController : BaseController
    {
        private readonly IssuesService _issueService;
        private readonly ProjectsService _projectsService;

        public IssuesController(IssuesService issuesService, ProjectsService projectsService)
        {
            _issueService = issuesService;
            _projectsService = projectsService;
        }

        // GET: api/projects/1/issues
        [HttpGet("{projectId}/issues")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Issue>>> Get(int projectId)
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
                return Forbid();
            }

            var issues = await _issueService.GetIssuesAsync(projectId);
            return Ok(issues);
        }

        // GET api/projects/1/issues/5
        [HttpGet("{projectId}/issues/{id}")]
        [Authorize]
        public async Task<ActionResult<Issue>> Get(int projectId, int id)
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
                return Forbid();
            }

            var issue = await _issueService.GetIssueAsync(id);
            return Ok(issue);
        }


        // POST api/projects/1/issues
        [HttpPost("{projectId}/issues")]
        [Authorize]
        public async Task<ActionResult<Issue>> Post(int projectId, [FromBody] CreateIssueModel createIssueModel)
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
                return Forbid();
            }

            var issue = new Issue
            {
                ProjectId = projectId,
                BoardId = createIssueModel.BoardId,
                Name = createIssueModel.Name,
                Description = createIssueModel.Description,
            };

            var createdIssue = await _issueService.CreateIssueAsync(issue);
            return Ok(createdIssue);
        }


        // PUT api/projects/1/issues/5
        [HttpPut("{projectId}/issues/{id}")]
        [Authorize]
        public async Task<ActionResult<Issue>> Put(int projectId, int id, [FromBody] Issue updatedIssue)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var existingIssue = await _issueService.GetIssueAsync(id);
            if (existingIssue == null)
            {
                return NotFound("Issue does not exist");
            }

            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound("Project does not exist");
            }

            if (!project.MemberUserIds.Contains((int)userId) && project.OwnerUserId != (int)userId)
            {
                return Forbid();
            }

            var result = await _issueService.UpdateIssueAsync(updatedIssue);
            if (result == null)
            {
                return BadRequest("Failed to update the issue.");
            }
            return Ok(result);
        }

        // DELETE api/projects/1/issues/5
        [HttpDelete("{projectId}/issues/{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int projectId, int id)
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
                return Forbid();
            }

            var existingIssue = await _issueService.GetIssueAsync(id);
            if (existingIssue == null)
            {
                return NotFound("Issue does not exist");
            }

            var success = await _issueService.DeleteIssueAsync(id);
            if (!success)
            {
                return BadRequest(false);
            }

            return Ok(true); // Typically, a 204 No Content response is used for DELETE success
        }
    }
}
