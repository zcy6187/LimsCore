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
        public const string SearchInput = "Pages.Assay.SearchInput";

        public const string AssaySearch = "Pages.AssaySearch";
        public const string SimpleSearch = "Pages.AssaySearch.SimpleSearch";
    }
}
