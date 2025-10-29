using Bulky.Models;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            new Category { Id = 2, Name = "Scı-Fi", DisplayOrder = 2 },
            new Category { Id = 3, Name = "History", DisplayOrder = 3 }
        );
    }
}