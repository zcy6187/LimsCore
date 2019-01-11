using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.SysManager.ZtManager.Dto
{
    [AutoMapTo(typeof(ZtCode))]
    public class EditZtCodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}
