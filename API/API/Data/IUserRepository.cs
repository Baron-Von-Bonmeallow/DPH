namespace API.Data
{
    public interface IUserRepository
    {
        string? GetUserById(System.IdentityModel.Tokens.Jwt.JwtSecurityToken token);
        string? GetUserRoleInEvent(System.IdentityModel.Tokens.Jwt.JwtSecurityToken token, Guid eventId);
    }
}
