using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Assay.Token))]
    public class EditTplToken
    {
        public int Id { get; set; }
        public string CmdToken { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string TplIds { get; set; }
        public string TplNames { get; set; }
        public string Contracter { get; set; }
        public string PhoneNumber { get; set; }
        public List<TokenTplDto> TokenTplList { get; set; }
    }

    public class TokenTplDto
    {
        public int Id { get; set; }
        public string TplName { get; set; }
    }
}
