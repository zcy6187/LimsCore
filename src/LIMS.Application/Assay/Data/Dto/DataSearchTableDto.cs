using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class DataSearchTableDto
    {
        public TemplateInfoDto TableHead { get; set; }
        public List<List<string>> TableData { get; set; }
    }
}
