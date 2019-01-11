using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using LIMS.Configuration.Dto;

namespace LIMS.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : LIMSAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
