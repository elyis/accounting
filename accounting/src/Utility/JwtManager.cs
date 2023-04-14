using accounting.src.Entity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace accounting.src.Utility
{
    public class JwtManager
    {
        private static string _key = "testy12%GDSA^7%#4323sfdgDAcz@#43";

        private static SigningCredentials _signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            SecurityAlgorithms.HmacSha256
            );

        public static string GenerateAccessToken(Guid id, string roleName)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", id.ToString()),
                new Claim(ClaimTypes.Role, roleName)
            };

            var token = new JwtSecurityToken(
                claims: claims, 
                expires: DateTime.UtcNow.AddHours(2), 
                signingCredentials: _signingCredentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static string GenerateRefreshToken()
            => Guid.NewGuid().ToString();

        public static TokenPair GenerateTokenPair(Guid id, string roleName)
            => new TokenPair
            {
                AccessToken = "Bearer " + GenerateAccessToken(id, roleName),
                RefreshToken = GenerateRefreshToken()
            };

        public static List<Claim> GetClaims(string token)
        {
            var tokenWithoutSchema = token.Replace("Bearer ", "");
            return new JwtSecurityTokenHandler()
                .ReadJwtToken(tokenWithoutSchema)
                .Claims.ToList();
        }

        public static Guid GetClaimId(string token)
        {
            var claims = GetClaims(token);
            string userId = claims.First(claim => claim.Type == "UserId")?.Value!;
            return Guid.Parse(userId);
        }
    }
}

