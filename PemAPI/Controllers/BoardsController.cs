using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using PemAPI.Models;
using PemAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PemAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class BoardsController : BaseController
    {
        private readonly BoardsService _boardService;
        private readonly ProjectsService _projectsService;
        private readonly IssuesService _issuesService;


        public BoardsController(BoardsService boardService, ProjectsService projectsService, IssuesService issuesService)
        {
            _boardService = boardService;
            _projectsService = projectsService;
            _issuesService = issuesService;
        }

        // GET: api/projects/1/boards
        [HttpGet("projects/{projectId}/boards")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Board>>> Get(int projectId)
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

            var boards = await _boardService.GetBoardsAsync((int)userId);
            return Ok(boards);
        }


        // GET api/projects/1/boards/5
        [HttpGet("projects/{projectId}/boards/{id}")]
        [Authorize]
        public async Task<ActionResult<Board>> Get(int projectId, int id)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var board = await _boardService.GetBoardAsync(id);
            if (board == null)
            {
                return NotFound("Board does not exist");
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

            return Ok(board);
        }



        // POST api/projects/1/boards
        [HttpPost("projects/{projectId}/boards")]
        [Authorize]
        public async Task<ActionResult<Board>> Post(int projectId, [FromBody] CreateBoardModel createBoardModel)
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

            var board = new Board
            {
                ProjectId = createBoardModel.ProjectId,
                Name = createBoardModel.Name
            };
            var createdBoard = await _boardService.CreateBoardAsync(board);
            return Ok(createdBoard);
        }


        // PUT api/projects/1/boards/5
        [HttpPut("projects/{projectId}/boards/{id}")]
        [Authorize]
        public async Task<IActionResult> Put(int projectId, int id, [FromBody] Board updatedBoard)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            var existingBoard = await _boardService.GetBoardAsync(id);
            if (existingBoard == null)
            {
                return NotFound("Board does not exist");
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

            var result = await _boardService.UpdateBoardAsync(updatedBoard);
            if (result == null)
            {
                return BadRequest("Failed to update the board.");
            }
            return Ok(result);
        }


        // DELETE api/projects/1/boards/5
        [HttpDelete("projects/{projectId}/boards/{id}")]
        [Authorize]
        public async Task<ActionResult<bool>> Delete(int projectId, int id, [FromBody] int boardIdToMoveIssuesTo)
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
            {
                return BadRequest("Invalid token.");
            }

            if (id == boardIdToMoveIssuesTo)
            {
                return BadRequest("Cannot move issues from same board to same board");
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

            var board = await _boardService.GetBoardAsync(id);
            if (board == null)
            {
                return NotFound("Board does not exist");
            }

            var boardTo = await _boardService.GetBoardAsync(boardIdToMoveIssuesTo);
            if (boardTo == null)
            {
                return NotFound("Board to move issues to does not exist");
            }

            // Check if there is more than one board for given project
            var boards = await _boardService.GetBoardsAsync(projectId);
            if (boards.Count() <= 1)
            {
                return Problem("Can not delete only board remaining for project");
            }

            // Get all issues on board
            var issues = await _issuesService.GetIssuesAsync(projectId);
            if (issues != null && issues.Any())
            {
                // Move issues to other board
                foreach (Issue issue in issues)
                {
                    issue.BoardId = boardIdToMoveIssuesTo;
                    var didSuccessfullyDeleteIssue = await _issuesService.UpdateIssueAsync(issue);
                    if (didSuccessfullyDeleteIssue == null) 
                    {
                        return StatusCode(500, "Could not move issue to other board");
                    }
                }
            }

            var success = await _boardService.DeleteBoardAsync(id);
            if (!success)
            {
                return NotFound(false);
            }
            return Ok(true);
        }

    }
}
