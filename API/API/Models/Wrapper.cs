using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class ProjectWrapper
    {
        [Required]
        public Projects Project { get; set; } = null!;
    }
}