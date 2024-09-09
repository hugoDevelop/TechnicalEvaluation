using Microsoft.EntityFrameworkCore;
using TechnicalEvaluationApiRest.Models;

namespace TechnicalEvaluationApiRest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Municipality> Municipalities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("countries", schema: "locations");
            modelBuilder.Entity<Department>().ToTable("departments", schema: "locations");
            modelBuilder.Entity<Municipality>().ToTable("municipalities", schema: "locations");
            modelBuilder.Entity<User>().ToTable("users", schema: "users");
        }
    }
}