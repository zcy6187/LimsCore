using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    [Table("sys_UserTpl")]
    public class UserTpl : Entity, ISoftDelete
    {
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string TplIds { get; set; }
        // 类型——0-输入，1—查看
        public int? Lx { get; set; }
        public bool IsDeleted { get;set; }
    }

    [Table("sys_userOrgTpl")]
    public class UserOrgTpl : Entity, ISoftDelete
    {
        public long UserId { get; set; }
        // 用户的化验模板ID
        public int OrgId { get; set; }
        public string TplIds { get; set; }
        public bool IsDeleted { get; set; }
    }

    [Table("sys_userOrg")]
    public class UserOrg : Entity, ISoftDelete
    {
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string OrgIds { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class V_Attendance: Entity
    {
        public string orgCode { get; set; }
        public string orgName { get; set; }
        public int tplId { get; set; }
        public string tplName { get; set; }
        public int tplSpecId { get; set; }
        public string tplSpecName { get; set; }
        public string tplElementNames { get; set; }
        public string man_banci { get; set; }
        public string man_luci { get; set; }
        public DateTime signTime { get; set; }
        public DateTime samplingdate { get; set; }
        public string Lx { get; set; }
        public string eleNames { get; set; }
        public int Flag { get; set; }
        public bool IsDeleted { get; set; }
        public string samplingTime { get; set; }
        public string selfCode { get; set; }
        public string description { get; set; }
        public string scanId { get; set; }
        public string elementIds { get; set; }
    }
    [Table("assay_selfTpl")]
    public class SelfTpl : Entity
    {
        public string tplName { get; set; }
        public long userId { get; set; }
        public string tplIds { get; set; }
        public string tplSpecIds { get; set; }
        public bool IsDeleted { get; set; }
    }
}
