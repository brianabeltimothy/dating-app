using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {   
        //hold the private key
        private readonly SymmetricSecurityKey _key;

        //generate the private key
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            //create the signature
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //payload(claims) and signature combined
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler(); 

            var token = tokenHandler.CreateToken(tokenDescriptor); //this actually create the method

            return tokenHandler.WriteToken(token);
        }
    }
}