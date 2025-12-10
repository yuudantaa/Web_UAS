using Microsoft.EntityFrameworkCore;
using TrainerCourse.Backend.Models;

namespace TrainerCourse.Backend.DbMapper
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Trainer> Trainers { get; set; } = null!;
    }
}
