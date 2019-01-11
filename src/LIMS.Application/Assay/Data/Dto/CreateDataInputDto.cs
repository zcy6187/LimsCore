using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class CreateDataInputDto
    {
        public int TplId { get; set; }
        public DateTime SamplingDate { get; set; }
        public DateTime? SignDate { get; set; }
        public string SamplingTime { get; set; }
        public string FormValue { get; set; }
    }

    public class SpecDataDto
    {
        public int SpecId { get; set; }
        public string SignId { get; set; }
        public string OldId { get; set; }
        public List<EleDataDto> EleDataList { get; set; }
    }

    public class EleDataDto
    {
        public string OldId { get; set; }
        public int SpecId { get; set; }
        public int EleId { get; set; }
        public string EleValue { get; set; }
        public string EleName { get; set; }
        public string OperatorId { get; set; }
    }
}
