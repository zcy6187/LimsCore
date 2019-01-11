using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using LIMS.Assay.Authorization;
using LIMS.Authorization;
using LIMS.SysManager.Authorization;

namespace LIMS
{
    [DependsOn(
        typeof(LIMSCoreModule),
        typeof(AbpAutoMapperModule))]
    public class LIMSApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<LIMSAuthorizationProvider>();
            Configuration.Authorization.Providers.Add<AssayAppAuthorizationProvider>();
            Configuration.Authorization.Providers.Add<SystemAppAuthorizationProvicer>();

            // 自定义类型映射
            Configuration.Modules.AbpAutoMapper().Configurators.Add(configuration =>
            {
                // XXXMapper.CreateMappers(configuration);


            });
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(LIMSApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddProfiles(thisAssembly)
            );
        }
    }
}
