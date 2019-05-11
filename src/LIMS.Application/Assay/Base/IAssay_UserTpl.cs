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

        Dtos.HtmlDataOperRetDto AddOrUpdateSingleTplSpec(TplSpecDto specItem);
        Dtos.HtmlDataOperRetDto AddOrUpdateUserOrg(UserDataDto input);

        Dtos.HtmlDataOperRetDto AddOrUpdateUserOrgs(string orgIds);
        Dtos.HtmlDataOperRetDto AddOrUpdateOrgTpls(int orgId, string tplIds);
        Dtos.HtmlDataOperRetDto AddOrUpdateTplSpecByTplId(int tplId, string specIds);

        List<string> GetUserOrgIds();
        List<string> GetUserTplIds();
        List<string> GetUserTplSpecIds(int tplId);
        List<string> GetUserTplIdsByOrgId(int orgId);
    }
}
