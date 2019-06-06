using Abp.Authorization;
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
            pageAssayInput.CreateChildPermission(AssayAppPermissions.XySignDataInput,L(AssayAppPermissions.XySignDataInput));

            var pageAssaySearch = context.CreatePermission(AssayAppPermissions.AssaySearch,
                L(AssayAppPermissions.AssaySearch),
                L(AssayAppPermissions.AssaySearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.SimpleSearch, L(AssayAppPermissions.SimpleSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.UserMultiTableSearch,L(AssayAppPermissions.UserMultiTableSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.ZtMultiTableSearch,L(AssayAppPermissions.ZtMultiTableSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.UserSingleTableSearch, L(AssayAppPermissions.UserSingleTableSearch));
            pageAssaySearch.CreateChildPermission(AssayAppPermissions.SelfTplSearch,L(AssayAppPermissions.SelfTplSearch));

            var pageStatistic = context.CreatePermission(AssayAppPermissions.Statistic,
                L(AssayAppPermissions.Statistic),
                L(AssayAppPermissions.Statistic));
            pageStatistic.CreateChildPermission(AssayAppPermissions.Company,L(AssayAppPermissions.Company));

            var process = context.CreatePermission(AssayAppPermissions.AssayProcess,
               L(AssayAppPermissions.AssayProcess),
               L(AssayAppPermissions.AssayProcess));
            pageStatistic.CreateChildPermission(AssayAppPermissions.ProcessFormula, L(AssayAppPermissions.ProcessFormula));
            pageStatistic.CreateChildPermission(AssayAppPermissions.ProcessConst, L(AssayAppPermissions.ProcessConst));

            var detectCenter = context.CreatePermission(AssayAppPermissions.AssayDetector,L(AssayAppPermissions.AssayDetector),L(AssayAppPermissions.AssayDetector));
            detectCenter.CreateChildPermission(AssayAppPermissions.DetectExcelImport,L(AssayAppPermissions.DetectExcelImport));
            detectCenter.CreateChildPermission(AssayAppPermissions.DetectDuplicationSearch,L(AssayAppPermissions.DetectDuplicationSearch));
            detectCenter.CreateChildPermission(AssayAppPermissions.DetectModificationSearch,L(AssayAppPermissions.DetectModificationSearch));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, LIMSConsts.LocalizationSourceName);
        }
    }
}
