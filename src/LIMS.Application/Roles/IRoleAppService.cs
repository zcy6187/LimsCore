using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LIMS.Roles.Dto;

namespace LIMS.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
