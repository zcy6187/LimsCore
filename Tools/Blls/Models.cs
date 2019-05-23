using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Blls
{
    public class UserTpl
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string TplIds { get; set; }
        // 类型——0-输入，1—查看
        public int? Lx { get; set; }
        public bool IsDeleted { get; set; }
    }
    [Table("Sys_UserOrgTpl")]
    public class UserOrgTpl
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string OrgId { get; set; }
        public string TplIds { get; set; }
        public bool IsDeleted { get; set; }
    }

    [Table("Sys_UserOrg")]
    public class UserOrg
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        // 用户的化验模板ID
        public string OrgIds { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class V_Attendance
    {
        public int Id { get; set; }
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

    public class SelfTpl
    {
        public int Id { get; set; }
        public string tplName { get; set; }
        public long userId { get; set; }
        public string tplIds { get; set; }
        public string tplSpecIds { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 元素表
    public class Element
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 样品表
    public class Specimen
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lx { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 计量单位表
    public class Unit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 化验模板表
    public class Template
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string TplName { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 模板样品表
    public class TplSpecimen
    {
        public int Id { get; set; }
        public int TplId { get; set; }
        public string TplName { get; set; }
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Lx { get; set; }
        public int OrderNum { get; set; }
        public bool IsUse { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }

        public bool IsDeleted { get; set; }
    }

    // 样品元素表
    public class TplElement
    {
        public int Id { get; set; }
        public int TplId { get; set; }
        public int TplSpecId { get; set; }
        public string TplName { get; set; }
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public int ElementId { get; set; }
        public string ElementName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int OrderNo { get; set; }
        public float MaxNum { get; set; }
        public float MinNum { get; set; }
        public bool IsUse { get; set; }
        public string RowNum { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime LastModifyTime { get; set; }
        public int CreatorId { get; set; }
        public string Creator { get; set; }

        public bool IsDeleted { get; set; }
    }

    // 样品签到表
    public class Attendance
    {
        public int Id { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Man_Banci { get; set; }
        public string Man_Luci { get; set; }
        public string Man_Pinci { get; set; }
        public DateTime SamplingDate { get; set; }
        public string SamplingTime { get; set; }
        public int TplId { get; set; }
        public string SpecId { get; set; }
        public string ElementIds { get; set; }
        public int TplSpecId { get; set; }
        public string TplElementIds { get; set; }
        public string TplElementNames { get; set; }
        public string SelfCode { get; set; }
        public string Lx { get; set; }
        public DateTime SignTime { get; set; }
        public string Description { get; set; }
        public string ScanId { get; set; }
        public bool IsDeleted { get; set; }
        public int Flag { get; set; }
        public string MainScanId { get; set; }
    }


    public class UserZt
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public int ZtId { get; set; }
        public string ZtCode { get; set; }
    }

    public class OrgInfo
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string FatherCode { get; set; }
        public int OrderNo { get; set; }
        public int Layer { get; set; }
        public string Lx { get; set; }
        public string OrgName { get; set; }
        public string AliasName { get; set; }
        public string FullName { get; set; }
        public bool IsUse { get; set; }
        public DateTime CreateTime { get; set; }
        public string OldId { get; set; }
    }
}
