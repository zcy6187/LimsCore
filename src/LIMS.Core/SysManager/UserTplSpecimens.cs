using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.SysManager
{
    [Table("sys_userTplSpecimen")]
    public class UserTplSpecimens:Entity,ISoftDelete
    {
        public long UserId { get; set; }
        public int TplId { get; set; }
        public string SpecimenIds { get; set; }
        public bool IsDeleted { get; set; }
    }
}
