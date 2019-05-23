using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LimsX.MiddleWare
{
    public static class UseLimsCookieAuthenticationMiddlewareExtension
    {
        public static IApplicationBuilder UseLimsCookieAuthentication(
           this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LimsCookieAuthenticationMiddleware>();
        }
    }
}
