using API.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Support
{
    public class Tokens
    {
        private readonly string _secretKey;
        private readonly IConfiguration _config;
        private readonly IHttpContextAccessor _http;

        public Tokens(IConfiguration config, IHttpContextAccessor http)
        {
            _config = config;
            _secretKey = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key is missing");
            _http = http;
        }

        public JwtSecurityToken? IsValidToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                token = token.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey)),
                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidAudience = _config["Jwt:Audience"],
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return (JwtSecurityToken)validatedToken;
            }
            catch
            {
                return null;
            }
        }

        public string? GetUserById(JwtSecurityToken token)
        {
            return token?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
        }

        private string? GetUserRoleInResource(JwtSecurityToken token, Guid resourceId, string resourceType, DataContext context)
        {
            var userId = GetUserById(token);
            if (userId == null) return null;

            var collaborator = context.Collaborators
                .FirstOrDefault(c =>
                    c.UserId == userId &&
                    c.ResourceId == resourceId &&
                    c.ResourceType == resourceType);

            return collaborator?.Role.ToString();
        }

        public string? GetRoleByToken(JwtSecurityToken token)
        {
            return token?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
        }

        public Dictionary<Guid, string> GetUserRolesForResources(JwtSecurityToken token, List<Guid> resourceIds, string resourceType, DataContext context)
        {
            var userId = GetUserById(token);
            if (userId == null) return new Dictionary<Guid, string>();

            return context.Collaborators
                .Where(c =>
                    c.UserId == userId &&
                    resourceIds.Contains(c.ResourceId) &&
                    c.ResourceType == resourceType)
                .ToDictionary(c => c.ResourceId, c => c.Role.ToString());
        }

        public bool IsValid(string token)
        {
            return IsValidToken(token) != null;
        }

        public string? GetUserEmail(JwtSecurityToken token)
        {
            return token?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        }

        public string GenerateToken(string userId, string username, string? role = null)
        {
            var claims = new List<Claim>
            {
                new Claim("userId", userId),
                new Claim(ClaimTypes.Name, username)
            };

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim("role", role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string? userId, string? role) GetUserInfoFromToken()
        {
            var token = _http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token)) return (null, null);

            var jwt = IsValidToken(token);
            if (jwt == null) return (null, null);

            var userId = GetUserById(jwt);
            var role = GetRoleByToken(jwt);

            return (userId, role);
        }
    }
}