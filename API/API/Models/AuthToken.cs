using API.Models;

public class AuthToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
    public Guid UserId { get; set; }
    public required Users User { get; set; }
}