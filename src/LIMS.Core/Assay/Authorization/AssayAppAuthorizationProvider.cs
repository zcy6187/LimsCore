﻿using Abp.Authorization;
using Abp.Localization;

namespace LIMS.Assay.Authorization
{
    public class AssayAppAuthorizationProvider:AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var pageAssayManager = context.CreatePermission(AssayAppPermissions.AssayManager,
                L(AssayAppPermissions.AssayManager), 
                L(AssayAppPermissions.AssayManager));

            pageAssayManager.CreateChildPermission(AssayAppPermissions.Element, L(AssayAppPermissions.Element));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.Specimen, L(AssayAppPermissions.Specimen));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.Template, L(AssayAppPermissions.Template));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.Unit, L(AssayAppPermissions.Unit));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.UserTpl, L(AssayAppPermissions.UserTpl));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.Token, L(AssayAppPermissions.Token));
            pageAssayManager.CreateChildPermission(AssayAppPermissions.Oper, L(AssayAppPermissions.Oper));

            var pageAssayInput = context.CreatePermission(AssayAppPermissions.AssayInput,
               L(AssayAppPermissions.AssayInput),
               L(AssayAppPermissions.AssayInput));
            pageAssayInput.CreateChildPermission(AssayAppPermissions.DataInput, L(AssayAppPermissions.DataInput));
            pageAssayInput.CreateChildPermission(AssayAppPermissions.SearchInput, L(AssayAppPermissions.SearchInput));

            var pageAssaySearch = context.CreatePermission(AssayAppPermissions.AssaySearch,
                L(AssayAppPermissions.AssaySearch),
                L(AssayAppPermissions.AssaySearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.SimpleSearch, L(AssayAppPermissions.SimpleSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.UserMultiTableSearch,L(AssayAppPermissions.UserMultiTableSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.ZtMultiTableSearch,L(AssayAppPermissions.ZtMultiTableSearch));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LIMSConsts.LocalizationSourceName);
        }
    }
}
