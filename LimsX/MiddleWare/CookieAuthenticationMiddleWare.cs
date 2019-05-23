using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimsX.MiddleWare
{
    public class LimsCookieAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public LimsCookieAuthenticationMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var url = context.Request.Path.Value;
            bool isLoginUrl = url == "/account/login";
            var user = context.User;
            var isAuthenticate = user?.Identity.IsAuthenticated ?? false;
            if (isLoginUrl || isAuthenticate)
            {
                return this._next(context);
            }
            else
            {
                return context.ChallengeAsync();
            }
        }
    }
}
