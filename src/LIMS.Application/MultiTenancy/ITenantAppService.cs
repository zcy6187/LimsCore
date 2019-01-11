using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LIMS.MultiTenancy.Dto;

namespace LIMS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
