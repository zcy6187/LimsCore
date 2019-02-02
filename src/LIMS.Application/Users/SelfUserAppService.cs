using Abp.AutoMapper;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using LIMS.Authorization.Users;
using LIMS.Users.Dto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIMS.Users
{
    public class SelfUserAppService: LIMSAppServiceBase,ISelfUserAppService
    {
        private readonly UserManager _userManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<User, long> _userRep;

        public SelfUserAppService(
            UserManager userManager,
            IPasswordHasher<User> passwordHasher,
            IRepository<User, long> userRep)
        {
            _userManager = userManager;
            this._passwordHasher = passwordHasher;
            this._userRep = userRep;
        }

        public async Task ChangePassword(ChangePasswordInput input)
        {
            input.OldPassword = input.OldPassword.Trim();
            input.NewPassword = input.NewPassword.Trim();

            //判断旧密码是否正确
            if (string.IsNullOrWhiteSpace(input.OldPassword) || string.IsNullOrWhiteSpace(input.NewPassword))
            {
                throw new UserFriendlyException("密码不能为空");
            }

            if (input.OldPassword == input.NewPassword)
            {
                throw new UserFriendlyException("新旧密码不能相同");
            }

            //获取abp用户
            var user = await _userManager.GetUserByIdAsync(AbpSession.UserId.Value);

            //判断新密码是否正确
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, input.OldPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UserFriendlyException("旧密码错误");
            }

            //新密码hash
            var hash = _passwordHasher.HashPassword(user, input.NewPassword);
            user.Password = hash;
            await _userManager.UpdateAsync(user);
        }

        public async Task ResetUserPassword(long uid)
        {
            //获取abp用户
            var user = await _userManager.GetUserByIdAsync(uid);

            //新密码hash
            var hash = _passwordHasher.HashPassword(user, "zcy123");
            user.Password = hash;
            await _userManager.UpdateAsync(user);
        }

        // 查询用户信息
        public List<UserDto> SearchUserByUserName(string input)
        {
            var users=this._userRep.GetAll().Where(x => x.UserName.Contains(input)).ToList();
            var userDtoList = users.MapTo<List<UserDto>>();

            return userDtoList;
        }
    }
}
