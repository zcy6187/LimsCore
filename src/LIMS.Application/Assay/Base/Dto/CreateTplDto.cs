using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Template))]
    public class CreateTplDto
    {
        public string OrgCode { get; set; }
        public string TplName { get; set; }
    }
}
