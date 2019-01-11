using System.Threading.Tasks;
using Abp.Application.Services;
using LIMS.Sessions.Dto;

namespace LIMS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
