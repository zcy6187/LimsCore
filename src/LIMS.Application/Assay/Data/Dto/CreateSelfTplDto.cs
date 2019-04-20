using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class SelfTplDto
    {
        public string tplId { get; set; }
        public string tplName { get; set; }
        public List<KeyValDto> specList { get; set; }
    }

    public class CreateSelfTplDto
    {
        public int id { get; set; }
        public string tplName { get; set; }
        public List<SelfTplDto> selfTpls { get; set; }
    }
}
