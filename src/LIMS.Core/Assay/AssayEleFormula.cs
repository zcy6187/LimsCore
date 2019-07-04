using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace LIMS.Assay
{
    [Table("assay_eleFormula")]
    public class AssayEleFormula : Entity, ISoftDelete
    {
        public int eleId { get; set; }
        public string name { get; set; }
        public string formulaExp { get; set; }
        [Column("isDeleted")]
        public bool IsDeleted { get; set; }
        public string intro { get; set; }
        public DateTime lastModifyTime { get; set; }
        public long operatorId { get; set; }
        public int flag { get; set; }
    }

    [Table("assay_formulaPrams")]
    public class AssayFormulaPram : Entity
    {
        public int formulaId { get; set; }
        public string pramName { get; set; }
        public string intro { get; set; }
    }

    [Table("assay_const")]
    public class AssayConst : Entity,ISoftDelete
    {
        public decimal constVal { get; set; }
        public int elementId { get; set; }
        public string intro { get; set; }
        public long operatorId { get; set; }
        public DateTime lastModifyTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
