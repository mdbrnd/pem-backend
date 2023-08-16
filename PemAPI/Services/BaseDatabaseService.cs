using PemAPI.Data;
using PemAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace PemAPI.Services
{
    public abstract class BaseDatabaseService
    {
        protected static PemDbContext CreateDbContext()
        {
            var connectionString = "Host=localhost;Database=postgres;User Id=postgres;Password=1234;";

            var optionsBuilder = new DbContextOptionsBuilder<PemDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            var db = new PemDbContext(optionsBuilder);

            return db;
        }
    }
}
