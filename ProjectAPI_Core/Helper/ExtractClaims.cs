using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ProjectAPI.Helper
{
    public class ExtractClaims
    {
        public static int? ExtractUserIdFromToken(string token)
        {
            try
            {
               
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                var userIdClaim = jwtToken.Claims.FirstOrDefault(t => t.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                return null; // Invalid token or claim not found
            }
            catch
            {
                return null; // Token parsing failed
            }
        }
    }
}
