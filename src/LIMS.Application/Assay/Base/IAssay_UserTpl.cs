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
    }
}
