using Microsoft.EntityFrameworkCore;
using PemAPI.Models;

namespace PemAPI.Data
{
    public class PemDbContext : DbContext
    {

        public PemDbContext()
        {
        }
        public PemDbContext(DbContextOptions<PemDbContext> options)
            : base(options)
        { }
        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Board> Boards { get; set; }

        public virtual DbSet<Issue> Issues { get; set; }

    }
}
