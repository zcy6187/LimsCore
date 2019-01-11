using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(Assay.UserTpl))]
    public class EditUserTplDto
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string TplIds { get; set; }
        // 类型——0-输入，1—查看
        public string Lx { get; set; }
        public bool IsDeleted { get; set; }
    }

    [AutoMapTo(typeof(Assay.V_UserTpl))]
    public class EditVUserTplDto
    {
        public long Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int? UserTplId { get; set; }
        // 用户的化验模板ID
        public string TplIds { get; set; }
        // 类型——0-输入，1—查看
        public int? Lx { get; set; }
        public bool? IsDeleted { get; set; }
        public string TplNames { get; set; }
        public List<TplDto> TplList {get;set;}
    }

    public class TplDto
    {
        public int Id { get; set; }
        public string TplName { get; set; }
    }
}
