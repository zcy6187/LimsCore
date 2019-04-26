using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMapTo(typeof(AssayEleFormula))]
    public class CreateFormulaDto
    {
        public int eleId { get; set; }
        public string name { get; set; }
        public string formulaExp { get; set; }
        public string intro { get; set; }
    }
}
