using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Statistic.Dto
{
    // 单个工厂的汇总信息
    public class PlantSummaryDto
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public List<SectionSummaryDto> SectionList{ get; set; }
    }

    // 单个工段的存储信息
    public class SectionSummaryDto
    {
        public string OrgCode { get; set; }
        public string OrgName { get; set; }
        public List<SpecSummaryDto> SpecList { get; set; }
    }

    // 单个样品的存储信息
    public class SpecSummaryDto
    {
        public string SpecName { get; set; }
        public int SpecCount { get; set; }
        public int EleCount { get; set; }
        public int AuAg { get; set; }
    }
}
