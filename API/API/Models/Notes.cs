namespace API.Models
{
    public class Notes
    {
        public Guid Id { get; internal set; }
        public required string Title { get; set; }
        public required string Content { get; set; }
        public required string OwnerId { get; set; }
        public required Users Owner { get; set; }
        public DateTime CreatedAt { get; internal set; }
        public DateTime UpdatedAt { get; internal set; }
    }
}
