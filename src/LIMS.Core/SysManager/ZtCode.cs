using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.SysManager
{
    [Table("sys_ZtCode")]
    public class ZtCode:Entity,ISoftDelete
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsDeleted { get; set; }
    }
}
