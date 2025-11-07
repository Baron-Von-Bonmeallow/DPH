namespace API.Models
{
    public class Reminders
    {
        public Guid Id { get; set; }

        public required string OwnerId { get; set; }

        public required string Message { get; set; }

        public required DateTime ReminderTime { get; set; }

        public bool Completed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}