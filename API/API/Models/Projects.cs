using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace API.Models
{
    public class Projects
    {

        public Guid Id { get; set; }= Guid.NewGuid();
        public required string Title { get; set; }
        public required string OwnerId { get; set; }
        public required Users Owner { get; set; }
        public string? Content { get; set; }
        public List<int>? UserIds { get; }
        

        //public SharedElement(int elementId, string content,List<int> userIds, SharedRole role)
        //{
        //    ElementId = elementId;
        //    Content = content;
        //    UserIds = userIds ?? new List<int>();
        //    Role = role;
        //}
    }

    //public enum SharedRole
    //{
    //    Owner,
    //    Admin,
    //    Editor,
    //    Viewer
    //}

}
