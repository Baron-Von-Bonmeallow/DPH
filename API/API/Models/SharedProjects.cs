namespace API.Models
{
    public class SharedProjects
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Projects? Project { get; set; }

        public required string UserId { get; set; }
        public required Users User { get; set; }
        //public SharedRole Role { get; }
    }
    //public enum SharedRole
    //{
    //    Owner,
    //    Admin,
    //    Editor,
    //    Viewer
    //}
}
