namespace Auth.Models
{
    public class Users
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? BirthDate { get; set; }
        public bool IsAdmin { get; set; }
        public string? PasswordHash { get; set; }
        public string? salt { get; set; }
        public DateFormat DFormat { get; set; } = DateFormat.YYYYMMDD;
    }
    public enum DateFormat
    {
        YYYYMMDD,
        DDMMYYYY,
        MMDDYYYY
    }
}
