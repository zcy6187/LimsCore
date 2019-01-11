using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMap(typeof(TplElement))]
    public class EditTplElementDto
    {
        public int Id { get; set; }
        public int TplId { get; set; }
        public string TplName { get; set; }
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public int ElementId { get; set; }
        public string ElementName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public int TplSpecId { get; set; }

        public float MaxNum { get; set; }
        public float MinNum { get; set; }
    }
}
