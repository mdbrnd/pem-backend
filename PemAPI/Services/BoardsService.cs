using Microsoft.EntityFrameworkCore;
using PemAPI.Data;
using PemAPI.Models;

namespace PemAPI.Services
{
    public class BoardsService : BaseDatabaseService
    {
        public async Task<Board?> GetBoardAsync(int boardId)
        {
            using PemDbContext _context = CreateDbContext();
            return await _context.Boards.SingleOrDefaultAsync(b => b.Id == boardId);
        }

        public async Task<IEnumerable<Board>> GetBoardsAsync(int projectId)
        {
            using PemDbContext _context = CreateDbContext();
            return await _context.Boards.Where(b => b.ProjectId == projectId).ToListAsync();
        }

        public async Task<Board> CreateBoardAsync(Board board)
        {
            using PemDbContext _context = CreateDbContext();
            await _context.Boards.AddAsync(board);
            await _context.SaveChangesAsync();
            return board;
        }

        public async Task<Board?> UpdateBoardAsync(Board board)
        {
            using PemDbContext _context = CreateDbContext();
            var existingBoard = await _context.Boards.SingleOrDefaultAsync(b => b.Id == board.Id);
            if (existingBoard == null)
            {
                return null;
            }

            // Update properties
            existingBoard.ProjectId = board.ProjectId;
            existingBoard.Name = board.Name;

            await _context.SaveChangesAsync();
            return existingBoard;
        }

        public async Task<bool> DeleteBoardAsync(int boardId)
        {
            using PemDbContext _context = CreateDbContext();
            var boardToDelete = await _context.Boards.SingleOrDefaultAsync(b => b.Id == boardId);
            if (boardToDelete == null)
            {
                return false;
            }

            _context.Boards.Remove(boardToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
