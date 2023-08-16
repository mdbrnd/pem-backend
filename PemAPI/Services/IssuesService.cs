using Microsoft.EntityFrameworkCore;
using PemAPI.Data;
using PemAPI.Models;

namespace PemAPI.Services
{
    public class IssuesService : BaseDatabaseService
    {
        public async Task<Issue?> GetIssueAsync(int issueId)
        {
            using PemDbContext _context = CreateDbContext();
            return await _context.Issues.SingleOrDefaultAsync(i => i.Id == issueId);
        }

        public async Task<IEnumerable<Issue>> GetIssuesAsync(int projectId)
        {
            using PemDbContext _context = CreateDbContext();
            return await _context.Issues.Where(i => i.ProjectId == projectId).ToListAsync();
        }

        public async Task<Issue> CreateIssueAsync(Issue issue)
        {
            using PemDbContext _context = CreateDbContext();
            await _context.Issues.AddAsync(issue);
            await _context.SaveChangesAsync();
            return issue;
        }

        public async Task<Issue?> UpdateIssueAsync(Issue issue)
        {
            using PemDbContext _context = CreateDbContext();
            var existingIssue = await _context.Issues.SingleOrDefaultAsync(i => i.Id == issue.Id);
            if (existingIssue == null)
            {
                return null;
            }

            // Update properties
            existingIssue.ProjectId = issue.ProjectId;
            existingIssue.BoardId = issue.BoardId;
            existingIssue.Completed = issue.Completed;
            existingIssue.Name = issue.Name;
            existingIssue.Description = issue.Description;

            await _context.SaveChangesAsync();
            return existingIssue;
        }

        public async Task<bool> DeleteIssueAsync(int issueId)
        {
            using PemDbContext _context = CreateDbContext();
            var issueToDelete = await _context.Issues.SingleOrDefaultAsync(i => i.Id == issueId);
            if (issueToDelete == null)
            {
                return false;
            }

            _context.Issues.Remove(issueToDelete);
            await _context.SaveChangesAsync();
            return true;
        }


    }
}
