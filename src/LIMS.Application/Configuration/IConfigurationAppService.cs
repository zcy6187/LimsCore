using System.Threading.Tasks;
using LIMS.Configuration.Dto;

namespace LIMS.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
