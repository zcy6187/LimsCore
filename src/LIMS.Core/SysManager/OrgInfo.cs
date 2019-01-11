using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.SysManager
{
    [Table("sys_OrgInfo")]
    public class OrgInfo:Entity
    {
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
