using Abp.Application.Services.Dto;
using LIMS.Assay.Data.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Data
{
    public interface IAssay_Attendance
    {
        PagedResultDto<AttendanceDto> GetAttendances(
            PagedResultRequestDto pageQueryDto, string orgCode,int? tplId,string specId,int flag, DateTime beginTime, DateTime endTime);
    }
}
