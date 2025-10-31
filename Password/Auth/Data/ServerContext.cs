using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Auth.Models;
namespace Auth.Data
{
    public class ServerContext:DbContext
    {
        public ServerContext(DbContextOptions<ServerContext> options)
            : base(options)
        {

        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Cookies> Cookies { get; set; }
    }
}
