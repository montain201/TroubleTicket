using Core.Helper;
using Core.Services;
using Core.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly JwtService jwtservice;
       
        private IUserService _userService;


        public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, JwtService jwtService, IUserService userService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtservice = jwtService;
            _userService = userService;
           
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterUser newUser)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser()
                {
                    UserName = newUser.UserName,
                    Email = "mon@123.com",
                };
                var result = await userManager.CreateAsync(user, newUser.Password);
                if (result.Succeeded)
                {
                    //کارشناس صرفا اجازه ثبت تیکت دارد
                    await userManager.AddToRoleAsync(user, "Expert");
                    await signInManager.SignInAsync(user, false);
                    return Ok("home");
                }
                else
                    return Ok(result.Errors);
            }
            return Ok("register");
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            if (ModelState.IsValid)
            {
                var jwt = _userService.Login(loginUser).GetAwaiter().GetResult();
                
                Response.Cookies.Append("jwt", jwt, new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });
                return Ok(new
                {
                    message = "success"
                });
            }
            else
                return Ok("Invalid UserName or Password");
        }




        //[HttpPost("Logout")]
        //public async Task<IActionResult> Logout()
        //{
        //    await signInManager.SignOutAsync();
        //    Response.Cookies.Delete("jwt", new CookieOptions()
        //    {
        //        HttpOnly = true,
        //        Secure = true,
        //        SameSite = SameSiteMode.None
        //    });
        //    return Ok("login");
        //}
        [HttpGet("User")]
        public async Task<IActionResult> User()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];
                var token = jwtservice.Verify(jwt);

                string userId = token.Issuer;
                var appUser = await signInManager.UserManager.FindByIdAsync(userId);
                return Ok(appUser);
            }
            catch (Exception e)
            {
                return Ok(Unauthorized());
            }
        }
    }
}
