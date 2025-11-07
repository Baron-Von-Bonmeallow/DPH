public class EventPermission
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public required string Role { get; set; } // Reader, Editor, Admin
}