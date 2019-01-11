using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    [AutoMapTo(typeof(TypeIn))]
    public class CreateTypeInDto
    {
        // 模板ID
        public int TplId { get; set; }
        public int SpecId { get; set; }
        // 签到表中的ID
        public string SignId { get; set; }
        public DateTime CreateTime { get; set; }
        // 数据录入者的用户ID
        public int CreatorId { get; set; }
        // 化验员
        public string Operator { get; set; }
        // 采样时间
        public string SamplingTime { get; set; }
        // 采样日期
        public string SamplingDate { get; set; }
        // 数据类型
        public string Lx { get; set; }
        // 是否平行样
        public bool IsParallel { get; set; }
        // 备注
        public string Remark { get; set; }
        // 平行样的主ID
        public string MainId { get; set; }
        public bool IsDeleted { get; set; }
        public string EleIds { get; set; }
        public string EleNames { get; set; }
    }

    [AutoMapTo(typeof(TypeInItem))]
    public class CreateTypeInItemDto
    {
        // TypeIn表中ID
        public int TypeInId { get; set; }
        public int TplId { get; set; }
        // Specimen的ID
        public int SpecimenId { get; set; }
        // Element的ID
        public int ElementId { get; set; }
        public string UnitName { get; set; }
        public string EleValue { get; set; }
        public bool IsDeleted { get; set; }
        public string Operator { get; set; }
        public string OperatorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
