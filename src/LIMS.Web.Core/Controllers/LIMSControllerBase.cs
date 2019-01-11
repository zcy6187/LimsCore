using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace LIMS.Controllers
{
    public abstract class LIMSControllerBase: AbpController
    {
        protected LIMSControllerBase()
        {
            LocalizationSourceName = LIMSConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
