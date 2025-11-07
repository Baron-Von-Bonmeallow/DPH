using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
        [NotMapped]
        public List<int>? UserIds { get; set; }
    }

}
