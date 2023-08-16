using Microsoft.EntityFrameworkCore;
using PemAPI.Data;
using PemAPI.Models;

namespace PemAPI.Services
{
    public class UserService : BaseDatabaseService
    {
        public async Task<User?> GetUserAsync(int id)
        {
            using PemDbContext _context = CreateDbContext();

            return await _context.Users.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using PemDbContext _context = CreateDbContext();

            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserAsync(string email)
        {
            using PemDbContext _context = CreateDbContext();

            var result = await _context.Users.SingleOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            return result;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            user.Id = 0; // = 0 then db will handle id
            using PemDbContext _context = CreateDbContext();

            await Console.Out.WriteLineAsync("Adding User");

            await _context.Users.AddAsync(user);

            await Console.Out.WriteLineAsync("Saving changes");
            await _context.SaveChangesAsync();
            return user;
        }


        public async Task<User?> UpdateUserAsync(User user)
        {
            using PemDbContext _context = CreateDbContext();

            // Fetch the existing user from the database.
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Id == user.Id);

            // If the user does not exist, return null.
            if (existingUser == null)
            {
                return null;
            }

            // Update the properties of the existing user with the provided user's values.
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.PasswordHash = user.PasswordHash;

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

            // Return the updated user.
            return existingUser;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using PemDbContext _context = CreateDbContext();

            // Fetch the user from the database.
            var userToDelete = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            // If the user does not exist, return false indicating failure.
            if (userToDelete == null)
            {
                return false;
            }

            // Remove the user from the DbContext.
            _context.Users.Remove(userToDelete);

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
