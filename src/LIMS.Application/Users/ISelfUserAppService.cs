using Abp.Application.Services;
using LIMS.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Users
{
   public interface ISelfUserAppService
    {
        Task ChangePassword(ChangePasswordInput input);
        Task ResetUserPassword(long uid);
        List<UserDto> SearchUserByUserName(string input);
    }
}
