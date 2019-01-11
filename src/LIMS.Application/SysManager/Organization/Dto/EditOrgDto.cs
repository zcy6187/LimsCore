using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LIMS.SysManager.Organization.Dto
{
    public class EditOrgDto
    {
        public int Id { get; set; }
        [Required]
        public string OrgName { get; set; }
        public string AliasName { get; set; }
        public string Code { get; set; }
    }
}
