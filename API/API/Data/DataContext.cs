using Microsoft.EntityFrameworkCore;
using API.Models;
namespace API.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Events> Events { get; set; }
        public DbSet<Notes> Notes { get; set; }
        public DbSet<Reminders> Reminders { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Projects> Projects { get; set; }
        public DbSet<SharedProjects> SharedProjects { get; set; }
        public DbSet<AuthToken> AuthTokens { get; set; }
        public DbSet<EventPermission> EventPermissions { get; set; }
        public DbSet<Collaborators> Collaborators { get; set; }

    }
}
