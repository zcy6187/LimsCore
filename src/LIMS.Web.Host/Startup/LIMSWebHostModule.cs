using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LIMS.Configuration;
using System.Web.Http.Cors;

namespace LIMS.Web.Host.Startup
{
    [DependsOn(
       typeof(LIMSWebCoreModule))]
    public class LIMSWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public LIMSWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            var cors = new EnableCorsAttribute("*", "*", "*");
            //GlobalConfiguration.Configuration.EnableCors(cors);
            //IocManager.RegisterAssemblyByConvention(typeof(LIMSWebHostModule).GetAssembly());
        }
    }
}
