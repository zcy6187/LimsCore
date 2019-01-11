using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(AssayUser))]
    public class EditAssayUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
    }
}
