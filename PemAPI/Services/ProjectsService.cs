using Microsoft.EntityFrameworkCore;
using PemAPI.Data;
using PemAPI.Models;

namespace PemAPI.Services
{
    public class ProjectsService : BaseDatabaseService
    {
        public async Task<Project?> GetProjectAsync(int id)
        {
            using PemDbContext _context = CreateDbContext();

            return await _context.Projects.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync(int userId)
        {
            using PemDbContext _context = CreateDbContext();

            return await _context.Projects.Where(p => p.MemberUserIds.Contains(userId) || p.OwnerUserId == userId).ToListAsync();
        }

        public async Task<Project> CreateProjectAsync(Project project)
        {
            using PemDbContext _context = CreateDbContext();

            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
            return project;
        }

        public async Task<Project?> UpdateProjectAsync(Project project)
        {
            using PemDbContext _context = CreateDbContext();

            // Fetch the existing project from the database.
            var existingProject = await _context.Projects.SingleOrDefaultAsync(p => p.Id == project.Id);

            // If the project does not exist, return null.
            if (existingProject == null)
            {
                return null;
            }

            // Update the properties of the existing project with the provided project's values.
            existingProject.Name = project.Name;
            existingProject.Description = project.Description;
            // Add other properties as needed.

            // Save changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // If there's an exception (e.g., a DB constraint is violated), return null.
                return null;
            }

            // Return the updated project.
            return existingProject;
        }

        public async Task<bool> DeleteProjectAsync(int projectId)
        {
            using PemDbContext _context = CreateDbContext();

            // Fetch the project from the database.
            var projectToDelete = await _context.Projects.SingleOrDefaultAsync(p => p.Id == projectId);

            // If the project does not exist, return false indicating failure.
            if (projectToDelete == null)
            {
                return false;
            }

            // Remove the project from the DbContext.
            _context.Projects.Remove(projectToDelete);

            // Save changes to the database.
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                // If there's an exception (e.g., a DB constraint is violated), return false indicating failure.
                return false;
            }

            // Return true indicating success.
            return true;
        }

    }
}
