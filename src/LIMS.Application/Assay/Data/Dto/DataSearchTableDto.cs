using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Data.Dto
{
    public class SelfSearchTableDto
    {
        public List<TemplateInfoDto> TableHead { get; set; }
        public List<List<string>> TableData { get; set; }
    }

    public class DataSearchTableDto
    {
        public TemplateInfoDto TableHead { get; set; }
        public List<List<string>> TableData { get; set; }
    }


    public class MultiTableDataInfoDto
    {
        public string TableTitle { get; set; }
        public List<string> TableHead { get; set; }
        public List<List<string>> TableData { get; set; }
        public List<StatisticDto> StatisticData { get; set; }
    }

    public class StatisticDto
    {
        public int EleId { get; set; }
        // 元素名称
        public string EleName { get; set; }
        // 行数
        public int TotalRowNum { get; set; }
        // 总数
        public double TotalValue { get; set; }
        // 平均数
        public double AvgValue { get; set; }
    }
}
