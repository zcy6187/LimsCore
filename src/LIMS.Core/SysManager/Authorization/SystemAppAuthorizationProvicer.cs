using Abp.Authorization;
using Abp.Localization;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.SysManager.Authorization
{
    public class SystemAppAuthorizationProvicer:AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pageSystem = context.CreatePermission(SystemAppPermissions.SystemManager,
                L(SystemAppPermissions.SystemManager),
                L(SystemAppPermissions.SystemManager));

            pageSystem.CreateChildPermission(SystemAppPermissions.User, L(SystemAppPermissions.User));
            pageSystem.CreateChildPermission(SystemAppPermissions.Role, L(SystemAppPermissions.Role));
            pageSystem.CreateChildPermission(SystemAppPermissions.Organization, L(SystemAppPermissions.Organization));
            pageSystem.CreateChildPermission(SystemAppPermissions.ZtCode, L(SystemAppPermissions.ZtCode));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LIMSConsts.LocalizationSourceName);
        }
    }
}
