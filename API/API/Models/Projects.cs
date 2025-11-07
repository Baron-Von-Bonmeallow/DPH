using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
namespace API.Models
{
    public class Projects
    {

        public Guid Id { get; set; }= Guid.NewGuid();
        [Required]
        public string Title { get; set; } = null!;
        public  string? OwnerId { get; set; }
        [JsonIgnore]
        public Users? Owner { get; set; }
        public string? Content { get; set; }
        public List<int>? UserIds { get; set; }
    }

}
