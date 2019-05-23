using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LimsX.Dal;
using LimsX.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace LimsX.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginUserDto UserInfo { get; set; }
        public string Message { get; set; }
        

        public void OnGet()
        {
            this.Message = string.Empty;
        }

        public async void OnPost()
        {
            if (string.IsNullOrEmpty(UserInfo.UserName) || string.IsNullOrEmpty(UserInfo.Userpassword))
            {
                this.Message = "0";
                return;
            }
            var item = CommonHelper.CheckLogin(UserInfo);
            if (item != null)
            {
                this.Message = "1";
            }
            else
            {
                this.Message = "-1";
            }
           
            if (item!=null)
            {
                var claimIdentity = new ClaimsIdentity("Cookie");
                claimIdentity.AddClaim(new Claim(ClaimTypes.Name,item.UserName.ToString()));
                claimIdentity.AddClaim(new Claim("UserInfo", JsonConvert.SerializeObject(item)));
                claimIdentity.AddClaim(new Claim("UserId",item.UserId.ToString()));

                var principal = new ClaimsPrincipal(claimIdentity);
                await HttpContext.SignInAsync(principal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddDays(10)
                    });

                if (string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                {
                    Response.Redirect("/Index");
                }
                else
                {
                    Response.Redirect(Request.Form["ReturnUrl"]);
                }
            }
        }
    }
}