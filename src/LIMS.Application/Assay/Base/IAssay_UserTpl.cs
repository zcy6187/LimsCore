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
        Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrg(UserDataDto input);

        Dtos.HtmlDataOperRetDto PostAddOrUpdateUserOrgs(string orgIds);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateOrgTpls(string orgId, string tplIds);
        Dtos.HtmlDataOperRetDto PostAddOrUpdateTplSpecByTplId(int tplId, string specIds);

        List<string> GetUserOrgIds();
        List<string> GetUserTplIds();
        List<string> GetUserTplSpecIds(int tplId);
        List<string> GetUserTplIdsByOrgId(int orgId);
        List<string> GetUserTplIdsByOrgCode(string orgCode);
    }
}
