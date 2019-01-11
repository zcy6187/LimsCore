using Abp.Application.Services.Dto;
using System;

namespace LIMS.SysManager.Organization.Dto
{
    public class OrgDto:EntityDto
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
