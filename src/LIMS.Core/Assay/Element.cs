using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    // 元素表
    [Table("assay_Element")]
    public class Element:Entity,ISoftDelete
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 样品表
    [Table("assay_Specimen")]
    public class Specimen:Entity,ISoftDelete
    {
        public string Name { get; set; }
        public string Lx { get; set; }
        public string Status { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 计量单位表
    [Table("assay_Unit")]
    public class Unit:Entity,ISoftDelete
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }

    // 化验模板表
    [Table("assay_Template")]
    public class Template:Entity,ISoftDelete
    {
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
    [Table("assay_TplSpecimen")]
    public class TplSpecimen:Entity,ISoftDelete
    {
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
    [Table("assay_TplElement")]
    public class TplElement:Entity,ISoftDelete
    {
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
    [Table("assay_Attendance")]
    public class Attendance:Entity,ISoftDelete
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string Man_Banci { get; set; }
        public string Man_Luci { get; set; }
        public string Man_Pinci { get; set; }
        public DateTime SamplingDate { get; set; }
        [StringLength(5)]
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
}
