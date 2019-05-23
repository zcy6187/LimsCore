using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    public class CreateConstDto
    {
        public int id { get; set; }
        public double constVal { get; set; }
        public string cType { get; set; }
        public string intro { get; set; }
        public long operatorId { get; set; }
    }
}
