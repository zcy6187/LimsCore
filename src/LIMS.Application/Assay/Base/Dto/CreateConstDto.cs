using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    public class CreateConstDto
    {
        public int id { get; set; }
        public decimal constVal { get; set; }
        public int elementId { get; set; }
        public string intro { get; set; }
        public long operatorId { get; set; }
    }
}
