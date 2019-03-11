using LIMS.Assay.Statistic.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace LIMS.Assay.Statistic
{
    public interface IAssay_Statistic
    {
        // 查询单个厂的统计信息（如果为工段，则查询其工厂名称，将工段包裹在工厂中）
        PlantSummaryDto GetPlantSummary(DateTime beginTime,DateTime endTime,string orgCode);
        // 查询单个公司的统计信息（如果为工段或工厂，将其包裹在公司中）
        List<PlantSummaryDto> GetCompanySummary(DateTime beginTime, DateTime endTime, string orgCode);
        // 下载Excel
        string GetExcel(DateTime beginTime, DateTime endTime, string orgCode);
    }
}
