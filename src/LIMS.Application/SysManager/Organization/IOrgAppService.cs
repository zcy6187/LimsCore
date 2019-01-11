using Abp.Application.Services;
using LIMS.SysManager.Organization.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.SysManager.Organization
{
    public interface IOrgAppService:IApplicationService
    {
        List<OrgTreeNodeDto> GetOrgTree();
        List<OrgTreeNodeDto> GetOrgTreeByRootCode(string code);
        List<OrgDto> GetOrgInfos();
        EditOrgDto GetSingleOrgInfo(int inputId);
        Task AddOrgInfo(CreateOrgDto input);
        Task DeleteOrgInfo(int inputId);
        Task EditOrgInfo(EditOrgDto input);

        List<OrgTreeNodeDto> GetOrgTreeByZtCode();
    }
}
