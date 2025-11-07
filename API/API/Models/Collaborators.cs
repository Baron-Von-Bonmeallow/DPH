namespace API.Models
{
    public class Collaborators
    {
        public Guid Id { get; set; }


        public Guid ResourceId { get; set; }
        public required string ResourceType { get; set; }

        public required string UserId { get; set; }
        public required Users User { get; set; }

        public SharedRole Role { get; set; }
    }
    public enum SharedRole
    {
        Owner,
        Admin,
        Editor,
        Viewer
    }
}