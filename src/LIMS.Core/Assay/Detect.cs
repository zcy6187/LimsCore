using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    [Table("assay_MaterialPrintData")]
    public class MaterialPrintData : Entity, ISoftDelete
    {
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
        public DateTime PrintTime { get; set; }
        public string Description { get; set; }
        public string ScanId { get; set; }
        public bool IsDeleted { get; set; }
        public int Flag { get; set; }
        public string MainScanId { get; set; }
        public string chs { get; set; }
        public string vendor { get; set; }
        public string fhtime { get; set; }
    }

    [Table("assay_detectMainInfo")]
    public class DetectMainInfo : Entity, ISoftDelete
    {
        public int specId { get; set; }
        public string specName { get; set; }
        public long operatorId { get; set; }
        public string mainScanId { get; set; }
        public int fhId { get; set; }
        public string chs { get; set; }
        public string verdor { get; set; }
        public string fhtime { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime createtime { get; set; }
        public DateTime modifyTime { get; set; }
    }

    [Table("assay_detectDuplicationInfoItems")]
    public class DuplicationInfoItems: Entity, ISoftDelete
    {
        public int mainId { get; set; }
        public string mainScanId { get; set; }
        public string scanId { get; set; }
        public int tplSpecId { get; set; }
        public string tplSpecName { get; set; }
        public int tplEleId { get; set; }
        public string tplEleName { get; set; }
        public string eleValue { get; set; }
        public int operId { get; set; }
        public string operName { get; set; }
        public DateTime modifyTime { get; set; }
        public long modifyUserId { get; set; }
        public string modifyUserName { get; set; }
        public bool IsDeleted { get; set; }
    }

    [Table("assay_detectMainInfoItems")]
    public class DetectMainInfoItems : Entity, ISoftDelete
    {
        public int mainId { get; set; }
        public string mainScanId { get; set; }
        public int tplSpecId { get; set; }
        public string tplSpecName { get; set; }
        public int tplEleId { get; set; }
        public string tplEleName { get; set; }
        public string eleValue { get; set; }
        public int operId { get; set; }
        public string operName { get; set; }
        public DateTime modifyTime { get; set; }
        public long modifyUserId { get; set; }
        public string modifyUserName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
