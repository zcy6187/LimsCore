using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    [Table("assay_Token")]
    public class Token:Entity,ISoftDelete
    {
        public string CmdToken { get; set; }
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public string TplIds { get; set; }
        public bool IsDeleted { get; set; }
        public string Contracter { get; set; }
        public string PhoneNumber { get; set; }
    }

}
