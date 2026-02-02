using Microsoft.EntityFrameworkCore;
using SportTracker.Models;

namespace SportTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Exercise> Exercises => Set<Exercise>();
}
