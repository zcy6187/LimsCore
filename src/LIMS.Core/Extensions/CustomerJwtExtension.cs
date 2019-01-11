using Abp.Dependency;
using Abp.Runtime.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LIMS.Extensions
{
    public static class CustomerJwtExtension
    {
        public static int GetZtCode(this IAbpSession abpSession)
        {
            var re = GetClaimValue(AbpClaimTypes.ZtCode);
            return string.IsNullOrEmpty(re) ? 0 : Convert.ToInt32(re);
        }

        private static string GetClaimValue(string claimType)
        {
            // 使用IOC容器获取当前用户身份认证信息 
            var PrincipalAccessor = IocManager.Instance.Resolve<IPrincipalAccessor>(); 
            var claimsPrincipal = PrincipalAccessor.Principal;
            var claim = claimsPrincipal?.Claims.FirstOrDefault(c => c.Type == claimType);
            if (string.IsNullOrEmpty(claim?.Value)) return null; return claim.Value;
        }
    }
}
