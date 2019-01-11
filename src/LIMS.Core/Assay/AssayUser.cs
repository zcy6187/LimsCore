using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    [Table("assay_User")]
    public class AssayUser:Entity,ISoftDelete
    {
        public string UserName { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
