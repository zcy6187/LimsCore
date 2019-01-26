using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using LIMS.Assay.Data.Dto;

namespace LIMS.Assay.Data
{
    public class Assay_AttendanceAppService : LIMSAppServiceBase, IAssay_Attendance
    {
        private IRepository<V_Attendance, int> _repository;

        public Assay_AttendanceAppService(IRepository<V_Attendance, int> repository)
        {
            _repository = repository;
        }

        // 分页查询数据
        // flag 未录入、部分录入、全部录入
        public PagedResultDto<AttendanceDto> GetAttendances(PagedResultRequestDto pageQueryDto, string orgCode, int? tplId, string specId, int flag, DateTime beginTime, DateTime endTime)
        {
            beginTime = DateTime.Parse(beginTime.ToString("yyyy-MM-dd 00:00:00"));
            endTime = DateTime.Parse(endTime.ToString("yyyy-MM-dd 23:59:59"));
            var query = _repository.GetAll().Where(x => !x.IsDeleted && x.signTime >= beginTime && x.signTime <= endTime);
            if (!string.IsNullOrEmpty(orgCode))
            {
                query = query.Where(x => x.orgCode.StartsWith(orgCode));
            }
            int typeTplId = tplId ?? 0;
            if (typeTplId > 0)
            {
                query = query.Where(x => x.tplId == typeTplId);
            }
            if (!string.IsNullOrEmpty(specId))
            {
                if (!specId.Contains("-1"))
                {
                    string[] eleIdArray = specId.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    int[] eleIdIntArray = Array.ConvertAll<string, int>(eleIdArray, s => int.Parse(s));
                    query = query.Where(x => eleIdIntArray.Contains(x.tplSpecId));
                }
            }
            if (flag < 3)
            {
                query = query.Where(x => x.Flag == flag);
            }


            var count = query.Count();

            var retList = new List<AttendanceDto>();
            if (count > 0)
            {
                var elements = query.OrderByDescending(x => x.Id)
                                .Skip(pageQueryDto.SkipCount)
                                .Take(pageQueryDto.MaxResultCount)
                                .ToList();
                foreach (var item in elements)
                {
                    var temp = new AttendanceDto()
                    {
                        Id = item.Id,
                        orgCode = item.orgCode,
                        orgName = item.orgName,
                        tplId = item.tplId,
                        tplName = item.tplName,
                        tplSpecId = item.tplSpecId,
                        tplSpecName = item.tplSpecName,
                        tplElementNames = item.tplElementNames,
                        man_banci = item.man_banci,
                        man_luci = item.man_luci,
                        signTime = item.signTime.ToString("yyyy-MM-dd"),
                        signDate = item.signTime,
                        samplingdate = item.samplingdate.ToString("yyyy-MM-dd"),
                        samplingTime=item.samplingdate.ToString("yyyy-MM-dd")+" "+item.samplingTime,
                        eleNames = item.eleNames,
                        Flag = GetFlagName(item.Flag),
                        selfCode=item.selfCode,
                        description=item.description,
                        scanId=item.scanId
                    };
                    retList.Add(temp);
                }
            }

            return new PagedResultDto<AttendanceDto>(
                count,
                retList
                );
        }

        private string GetFlagName(int flag)
        {
            string temp = string.Empty;
            switch (flag)
            {
                case 0:
                    temp = "未录入";
                    break;
                case 1:
                    temp = "部分录入";
                    break;
                case 2:
                    temp = "全部录入";
                    break;
            }
            return temp;
        }
    }
}
