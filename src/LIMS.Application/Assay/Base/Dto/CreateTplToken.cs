using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Assay.Token))]
    public class CreateTplToken
    {
        [Required]
        public string CmdToken { get; set; }
        public string OrgCode { get; set; }
        public string TplIds { get; set; }
        public string Contracter { get; set; }
        public string PhoneNumber { get; set; }
    }
}
