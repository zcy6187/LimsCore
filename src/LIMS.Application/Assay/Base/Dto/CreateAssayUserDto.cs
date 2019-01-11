using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(AssayUser))]
    public class CreateAssayUserDto
    {
        public string UserName { get; set; }
    }
}
