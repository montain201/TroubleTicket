using Core.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Core.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public UserService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public async Task<string> Login(LoginUser loginUser)
        {
            var identityResult = await signInManager.PasswordSignInAsync(loginUser.UserName, loginUser.Password, true, false);

            if (identityResult.Succeeded)
            {
                var appUser = await signInManager.UserManager.FindByNameAsync(loginUser.UserName);
                var role = userManager.GetRolesAsync(appUser);
                var lastrole = role.Result.FirstOrDefault();

                PasswordVerificationResult passresult = signInManager.UserManager.PasswordHasher.VerifyHashedPassword(appUser, appUser.PasswordHash, loginUser.Password);
                if (passresult == PasswordVerificationResult.Success)
                {   ///////////////////////////////
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(Startup.SECRET);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                                    new Claim(ClaimTypes.Name, loginUser.UserName+";"+appUser.Id),
                                    new Claim(ClaimTypes.Role, lastrole)
                        }),

                        Expires = DateTime.UtcNow.AddYears(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    string jwt = tokenHandler.WriteToken(token);
                    ////////////////////////
                    return await Task.Delay(1000).ContinueWith(t => jwt);
                  
                }
                else
                    return ("Invalid UserName or Password");
            }



            else
            {
                return ("login");
            }
        }


    }
}
