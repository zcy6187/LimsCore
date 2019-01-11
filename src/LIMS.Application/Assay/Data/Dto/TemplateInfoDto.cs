using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class TemplateInfoDto
    {
       public BaseInfoDto Template { get; set; }
       public List<BaseInfoDto> Specimens { get; set; }
        public List<BaseInfoDto> Elements { get; set; }
    }

    public class BaseInfoDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public int OrderNum { get; set; }
    }
}
