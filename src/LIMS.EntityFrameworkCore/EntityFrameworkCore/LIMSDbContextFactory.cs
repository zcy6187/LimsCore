using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using LIMS.Configuration;
using LIMS.Web;

namespace LIMS.EntityFrameworkCore
{
    /* This class is needed to run "dotnet ef ..." commands from command line on development. Not used anywhere else */
    public class LIMSDbContextFactory : IDesignTimeDbContextFactory<LIMSDbContext>
    {
        public LIMSDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<LIMSDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            LIMSDbContextConfigurer.Configure(builder, configuration.GetConnectionString(LIMSConsts.ConnectionStringName));

            return new LIMSDbContext(builder.Options);
        }
    }
}
