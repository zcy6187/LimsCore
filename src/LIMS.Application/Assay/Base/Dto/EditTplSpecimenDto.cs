using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Base.Dto
{
    [AutoMap(typeof(TplSpecimen))]
    public class EditTplSpecimenDto
    {
        public int Id { get; set; }
        public int TplId { get; set; }
        public string TplName { get; set; }
        public int SpecId { get; set; }
        public string SpecName { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; }
    }
}
