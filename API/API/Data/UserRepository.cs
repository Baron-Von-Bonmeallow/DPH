using System.IdentityModel.Tokens.Jwt;

namespace API.Data
{
    public class UserRepository
    {
        private readonly DataContext _context;
        public UserRepository(DataContext context)
        {
            _context = context;
        }
        // Add user-related data access methods here
        public string? GetUserById(JwtSecurityToken token)
        {
            return token?.Claims.FirstOrDefault(c => c.Type == "userId" || c.Type == "sub")?.Value;
        }
        public string? GetByName(string name)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == name);
            return user?.Id;
        }
        public string? GetUserRoleInResource(JwtSecurityToken token, Guid resourceId, string resourceType)
        {
            var userId = GetUserById(token);
            if (userId == null) return null;

            var collaborator = _context.Collaborators
                .FirstOrDefault(c =>
                    c.UserId == userId &&
                    c.ResourceId == resourceId &&
                    c.ResourceType == resourceType);

            return collaborator?.Role.ToString();
        }

    }
}
