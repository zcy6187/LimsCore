using LIMS.Assay.Base.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Assay.Base
{
    public interface IAssay_UserTpl
    {
        List<EditVUserTplDto> SearchUserTpls(string input);
        Task AddUserTpl(CreateUserTplDto input);
        Task EditUserTpl(EditVUserTplDto input);
        Task DeleteUserTplById(int inputId);

        Dtos.HtmlDataOperRetDto PostAddOrUpdateSingleTplSpec(TplSpecDto specItem);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrgs(string orgIds);

        Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrg(UserDataDto input);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrgsByUserId(string orgIds, long uid);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateOrgTpls(string orgId, string tplIds);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateOrgTplsByUserId(string orgId, string tplIds, long uid);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateTplSpecByTplId(int tplId, string specIds);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateTplSpecByTplIdAndUserId(int tplId, string specIds, long uid);

        List<string> GetUserOrgIdsByUserId(long userId);
        List<string> GetUserOrgIds();
        List<string> GetUserTplIdsByUserId(long userId);
        List<string> GetUserTplIds();
        List<string> GetUserTplSpecIdsByUserId(int tplId, long userId);
        List<string> GetUserTplSpecIds(int tplId);
        List<string> GetUserTplIdsByOrgId(int orgId);
        List<string> GetUserTplIdsByOrgCode(string orgCode);
        List<string> GetUserTplIdsByOrgCodeAndUserId(string orgCode, long userId);
    }
}
