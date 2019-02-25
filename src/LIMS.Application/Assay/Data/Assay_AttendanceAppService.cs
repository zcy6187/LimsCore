using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using LIMS.Assay.Data.Dto;
using LIMS.SysManager;

namespace LIMS.Assay.Data
{
    public class Assay_AttendanceAppService : LIMSAppServiceBase, IAssay_Attendance
    {
        private IRepository<V_Attendance, int> _repository;
        private IRepository<UserZt, int> _userZtRepository; 

        public Assay_AttendanceAppService(IRepository<V_Attendance, int> repository, IRepository<UserZt, int> userZtRepository)
        {
            _repository = repository;
            _userZtRepository = userZtRepository;
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

        // 按自定义编码查询签到信息
        public PagedResultDto<AttendanceDto> GetAttendancesBySelfCode(PagedResultRequestDto pageQueryDto,DateTime? beginTime,DateTime? endTime,string selfCode,int flag)
        {
            // 基本查询
            var query = _repository.GetAll().Where(x => !x.IsDeleted && x.selfCode.StartsWith(selfCode));
            if (beginTime != null)
            {
                beginTime = DateTime.Parse(((DateTime)beginTime).ToString("yyyy-MM-dd 00:00:00"));
                query = query.Where(x => x.signTime >= beginTime);
            }
            if (endTime != null)
            {
                endTime = DateTime.Parse(((DateTime)endTime).ToString("yyyy-MM-dd 00:00:00"));
                query = query.Where(x => x.signTime <= endTime);
            }
            if (flag < 3)
            {
                query = query.Where(x => x.Flag == flag);
            }

            // 添加权限
            long uid = AbpSession.UserId??0;
            List<string> ztCodeList=_userZtRepository.GetAll().Where(x => x.UserId == uid).Select(x => x.ZtCode).ToList();
            if (ztCodeList.Count() > 0)
            {
                if (ztCodeList.Count() == 1)
                {
                    query=query.Where(x => x.orgCode.StartsWith(ztCodeList[0]));
                }
                if (ztCodeList.Count() == 2)
                {
                    query=query.Where(x => x.orgCode.StartsWith(ztCodeList[0]) || x.orgCode.StartsWith(ztCodeList[1]));
                }
                if (ztCodeList.Count() == 3)
                {
                    query=query.Where(x => x.orgCode.StartsWith(ztCodeList[0]) || x.orgCode.StartsWith(ztCodeList[1])
                    || x.orgCode.StartsWith(ztCodeList[2]));
                }
            }

            int count = query.Count();
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
                        samplingTime = item.samplingdate.ToString("yyyy-MM-dd") + " " + item.samplingTime,
                        eleNames = item.eleNames,
                        Flag = GetFlagName(item.Flag),
                        selfCode = item.selfCode,
                        description = item.description,
                        scanId = item.scanId
                    };
                    retList.Add(temp);
                }
            }

            return new PagedResultDto<AttendanceDto>(
                count,
                retList
                );
        }

        // 综合类查询
        public PagedResultDto<AttendanceDto> GetAttendancesInfo(PagedResultRequestDto pageQueryDto, string orgCode, int? tplId, string specId, int flag, DateTime? beginTime, DateTime? endTime,string selfCode)
        {
            if (string.IsNullOrEmpty(selfCode))
            {
                return GetAttendances(pageQueryDto,orgCode,tplId,specId,flag, (DateTime)beginTime, (DateTime)endTime);
            }
            else
            {
                return GetAttendancesBySelfCode(pageQueryDto,beginTime,endTime,selfCode,flag);
            }
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

    public static class Utility
    {

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {

            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);

        }



        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }



        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }

    }

    public class ParameterRebinder : ExpressionVisitor
    {

        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }

    }
}
