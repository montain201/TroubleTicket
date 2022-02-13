using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Helper
{
    public class JwtService
    {
        private string secureKey = "this is a key secured very wll;";
        public string Generate(string id)
        {
            var symetricSercurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secureKey));
            var credentials = new SigningCredentials(symetricSercurityKey,SecurityAlgorithms.HmacSha256Signature);
            var header = new JwtHeader(credentials);
            
            var payload = new JwtPayload(id, null, null,null, DateTime.Today.AddDays(1));
            var securityToken = new JwtSecurityToken(header,payload);

            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }

        public JwtSecurityToken Verify(string jwt)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Startup.SECRET);

            tokenHandler.ValidateToken(jwt, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken securityToken);
            return (JwtSecurityToken)securityToken;


            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(secureKey);
            //tokenHandler.ValidateToken(jwt,new TokenValidationParameters { 
            //IssuerSigningKey = new SymmetricSecurityKey(key),
            //ValidateIssuerSigningKey = true,
            //ValidateIssuer=false,
            //ValidateAudience = false
            //},out SecurityToken securityToken);
            //return (JwtSecurityToken) securityToken;
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.ASCII.GetBytes(secureKey);
            //tokenHandler.ValidateToken(jwt,new TokenValidationParameters { 
            //IssuerSigningKey = new SymmetricSecurityKey(key),
            //ValidateIssuerSigningKey = true,
            //ValidateIssuer=false,
            //ValidateAudience = false
            //},out SecurityToken securityToken);
            //return (JwtSecurityToken) securityToken;
        }
    }

}
