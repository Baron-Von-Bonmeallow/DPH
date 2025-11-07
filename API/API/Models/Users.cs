using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace API.Models
{
    public class Users
    {
        public required string Id { get; set; }
        public string? UserName { get; set; }
        //public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Salt { get; set; }
        public ICollection<Projects>? OwnedProjects { get; set; }= new List<Projects>();
        public ICollection<SharedProjects>? SharedProjects { get; set; }= new List<SharedProjects>();

    }
}
