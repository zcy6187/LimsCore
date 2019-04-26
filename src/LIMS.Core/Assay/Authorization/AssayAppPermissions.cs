using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Authorization
{
    public static class AssayAppPermissions
    {
        /// <summary>
        ///  页面权限
        /// </summary>
        public const string AssayManager = "Pages.Assay";
        public const string Element = "Pages.Assay.Element";
        public const string Specimen = "Pages.Assay.Specimen";
        public const string Unit = "Pages.Assay.Unit";
        public const string UserTpl = "Pages.Assay.UserTpl";
        public const string Template = "Pages.Assay.Template";
        public const string Token = "Pages.Assay.Token";
        public const string Oper = "Pages.Assay.Oper";

        public const string AssayInput = "Pages.AssayInput";
        public const string DataInput = "Pages.AssayInput.DataInput";
        public const string SearchInput = "Pages.AssayInput.SearchInput";
        public const string XySignDataInput = "Pages.AssayInput.XySignDataInput";

        public const string AssaySearch = "Pages.AssaySearch";
        public const string SimpleSearch = "Pages.AssaySearch.SimpleSearch";
        public const string UserMultiTableSearch = "Pages.AssaySearch.UserMultiSearch";
        public const string UserSingleTableSearch = "Pages.AssaySearch.UserSingleTableSearch";
        public const string ZtMultiTableSearch = "Pages.AssaySearch.ZtMultiSearch";
        public const string SelfTplSearch = "Pages.AssaySearch.SelfTplSearch";

        public const string AssayProcess = "Pages.Process";
        public const string ProcessInstrument = "Pages.Process.Instrument";
        public const string ProcessFormula = "Pages.Process.Formula";


        public const string Statistic = "Pages.Statistic";
        public const string Company = "Pages.Statistic.Company";
    }
}
