using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.IdentityFramework;
using Abp.Localization;
using Abp.Runtime.Session;
using LIMS.Authorization;
using LIMS.Authorization.Roles;
using LIMS.Authorization.Users;
using LIMS.Roles.Dto;
using LIMS.Users.Dto;
using LIMS.SysManager;
using Abp.Domain.Uow;
using Abp.UI;

namespace LIMS.Users
{
    [AbpAuthorize(PermissionNames.Pages_Users)]
    public class UserAppService : AsyncCrudAppService<User, UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>, IUserAppService
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly IRepository<Role> _roleRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<UserZt, int> _userZtRepository;

        public UserAppService(
            IRepository<User, long> repository,
            UserManager userManager,
            RoleManager roleManager,
            IRepository<Role> roleRepository,
            IPasswordHasher<User> passwordHasher,
            IRepository<UserZt, int> userZtRepository)
            : base(repository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            this._userZtRepository = userZtRepository;
        }

        public override async Task<UserDto> Create(CreateUserDto input)
        {
            CheckCreatePermission();

            var user = ObjectMapper.Map<User>(input);

            user.TenantId = AbpSession.TenantId;
            user.Password = _passwordHasher.HashPassword(user, input.Password);
            user.IsEmailConfirmed = true;

            CheckErrors(await _userManager.CreateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            if (input.ZtCodes != null)
            {
                if (input.ZtCodes.Count > 0)
                {
                    SetUserZt(user.Id,input.ZtCodes);
                }
            }

            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(user);
        }

        public override async Task<UserDto> Update(UserDto input)
        {
            CheckUpdatePermission();

            var user = await _userManager.GetUserByIdAsync(input.Id);

            MapToEntity(input, user);

            CheckErrors(await _userManager.UpdateAsync(user));

            if (input.RoleNames != null)
            {
                CheckErrors(await _userManager.SetRoles(user, input.RoleNames));
            }

            if (input.ZtCodes != null)
            {
                if (input.ZtCodes.Count > 0)
                {
                    SetUserZt(user.Id, input.ZtCodes);
                }
            }

            return await Get(input);
        }

        public override async Task Delete(EntityDto<long> input)
        {
            var user = await _userManager.GetUserByIdAsync(input.Id);
            await _userManager.DeleteAsync(user);
        }

        public async Task<ListResultDto<RoleDto>> GetRoles()
        {
            var roles = await _roleRepository.GetAllListAsync();
            return new ListResultDto<RoleDto>(ObjectMapper.Map<List<RoleDto>>(roles));
        }

        public async Task ChangeLanguage(ChangeUserLanguageDto input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                AbpSession.ToUserIdentifier(),
                LocalizationSettingNames.DefaultLanguage,
                input.LanguageName
            );
        }

        protected override User MapToEntity(CreateUserDto createInput)
        {
            var user = ObjectMapper.Map<User>(createInput);
            user.SetNormalizedNames();
            return user;
        }

        protected override void MapToEntity(UserDto input, User user)
        {
            ObjectMapper.Map(input, user);
            user.SetNormalizedNames();
        }

        protected override UserDto MapToEntityDto(User user)
        {
            var roles = _roleManager.Roles.Where(r => user.Roles.Any(ur => ur.RoleId == r.Id)).Select(r => r.NormalizedName);
            var userDto = base.MapToEntityDto(user);
            userDto.RoleNames = roles.ToArray();
            return userDto;
        }

        protected override IQueryable<User> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAllIncluding(x => x.Roles);
        }

        protected override async Task<User> GetEntityByIdAsync(long id)
        {
            var user = await Repository.GetAllIncluding(x => x.Roles).FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                throw new EntityNotFoundException(typeof(User), id);
            }

            return user;
        }

        protected override IQueryable<User> ApplySorting(IQueryable<User> query, PagedResultRequestDto input)
        {
            return query.OrderBy(r => r.UserName);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }


        // 设置用户帐套
        [UnitOfWork]
        private void SetUserZt(long userId, List<UserZtDto> ztList)
        {
            // 批量刪除
            var ztUserList = _userZtRepository.GetAll().Where(x => x.UserId == userId);
            foreach (var userItem in ztUserList)
            {
                _userZtRepository.Delete(userItem);
            }
            // 批量添加
            foreach (var ztItem in ztList)
            {
                _userZtRepository.Insert(new UserZt()
                {
                    ZtCode = ztItem.ZtCode,
                    ZtId = ztItem.ZtId,
                    UserId = userId
                });
            }
        }

        public List<UserZtDto> GetUserZt(long userId)
        {
            List<UserZtDto> userZtList = new List<UserZtDto>();
            var ztUserList = _userZtRepository.GetAll().Where(x => x.UserId == userId).ToList();
            foreach (var item in ztUserList)
            {
                userZtList.Add(new UserZtDto()
                {
                    ZtCode = item.ZtCode,
                    ZtId = item.ZtId
                });
            }

            return userZtList;
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
    }
}
