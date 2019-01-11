using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.SysManager.Organization.Dto
{
    public class CreateOrgDto
    {
        [Required]
        public string OrgName { get; set; }
        public string AliasName { get; set; }
        [Required]
        public int ParentId { get; set; }
    }
}
