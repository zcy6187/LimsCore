using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.SysManager
{
    [Table("sys_UserZt")]
    public class UserZt:Entity
    {
        public long UserId { get; set; }
        public int ZtId { get; set; }
        public string ZtCode { get; set; }
    }
}
