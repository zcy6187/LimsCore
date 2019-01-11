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
        public PagedResultDto<AttendanceDto> GetAttendances(PagedResultRequestDto pageQueryDto, string orgCode, int? tplId, int? specId, int flag, DateTime beginTime, DateTime endTime)
        {
            var query = _repository.GetAll().Where(x => !x.IsDeleted && x.signTime >= beginTime && x.signTime <= endTime);
            if (!string.IsNullOrEmpty(orgCode))
            {
                query = query.Where(x => x.orgCode.StartsWith(orgCode));
            }
            int typeTplId = tplId ?? 0;
            int typeSpecId = specId ?? 0;
            if (typeTplId > 0)
            {
                query = query.Where(x => x.tplId == typeTplId);
            }
            if (typeSpecId > 0)
            {
                query = query.Where(x => x.tplSpecId == typeSpecId);
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
                        Lx = item.Lx,
                        eleNames = item.eleNames,
                        Flag = GetFlagName(item.Flag)
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
