using System.ComponentModel.DataAnnotations;

namespace API.ViewModels
{
    public class RegistryViewModel
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string? BirthDate { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
}
