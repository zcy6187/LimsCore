using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(UserTpl))]
    public class CreateUserTplDto
    {
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string TplIds { get; set; }
        // 类型——0-输入，1—查看
        public int? Lx { get; set; }
        public List<TplSpecimenDto> specimens{get;set;}
    }
}
