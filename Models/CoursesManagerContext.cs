using CoursesManager.Models;
using Microsoft.EntityFrameworkCore;

public class CoursesManagerContext : DbContext
{
    public CoursesManagerContext(DbContextOptions<CoursesManagerContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>()
            .HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId);

        modelBuilder.Entity<Course>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
}
