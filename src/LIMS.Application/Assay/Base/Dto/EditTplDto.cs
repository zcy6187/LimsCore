using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMap(typeof(Template))]
    public class EditTplDto
    {
        [Required]
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string TplName { get; set; }
    }
}
