using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace GameLobby.BL
{
    public class JwtService
    {
        public const string Issuer = "GameLobbyService";
        public const string Audience = "https://localhost:5000/";
        public const string Secret = "kocF1WYOrNdpd2wnXcTWtsT/9yRYeoOyf3B76gGYXSzm7GvH0j4hhPmVfN+1o6k2IZ5SHWh16wmQQJHvKsnIFA==";
        private const int ExpireMinutes = 30;

        public const string PlayerIdClaim = "player_id";
        public const string LobbyCodeClaim = "lobby_code";

        public string GenerateToken(IEnumerable<Claim> claims, int expireMinutes = ExpireMinutes)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = Issuer,
                Audience = Audience,
                Expires = now.AddDays(Convert.ToInt32(expireMinutes)),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            return token;
        }
    }
}
