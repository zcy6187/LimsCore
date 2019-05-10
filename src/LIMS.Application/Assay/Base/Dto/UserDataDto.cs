using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    public class UserDataDto
    {
        public string OrgIds { get; set; }
        public string TplIds { get; set; }
        public List<TplSpecDto> TplSpecList { get; set; } 
    }

    public class TplSpecDto
    {
        public string TplId { get; set; }
        public string SpecIds { get; set; }
    }
}
