using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialApp.Services
{
    public static class JwtService
    {
        public static string GenerateJwtToken(Claim[] additionalClaims, int tokenLifeTime)
        {
            var claims = new[]
            {
                // this guarantees the token is unique
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var claimList = new List<Claim>(claims);

            claimList.AddRange(additionalClaims);

            claims = claimList.ToArray();

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes("SSomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeyomeRandomKey");

            var securityToken = new JwtSecurityToken(
                expires: DateTime.UtcNow.AddMinutes(tokenLifeTime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                claims: claims
            );

            return tokenHandler.WriteToken(securityToken);
        }

        public static JwtSecurityToken? ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes("SSomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeySomeRandomKeyomeRandomKey");
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "UserId").Value);

                return jwtToken;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
