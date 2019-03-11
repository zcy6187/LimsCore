using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    /* FormName用于动态表单时，生成FormControl字段 */
    public class TemplateSchemaInputDto
    {
        public int TplId { get; set; }
        public string TplName { get; set; }
        public string FormName { get; set; }
        public List<SpecInputDto> SpecList { get; set; }
    }

    public class SpecInputDto
    {
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public string FormName { get; set; }
        public List<ElementInputDto> EleList { get; set; }
    }

    public class ElementInputDto
    {
        public int TplId{get;set;}
        public int SpecId { get; set; }
        public int EleId { get; set; }
        public string FormName { get; set; }
        public string EleName { get; set; }
        public string UnitName { get; set; }
        public string EleValue { get; set; }
        public bool IsVisible { get; set; }
    }
}
