using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace LIMS.EntityFrameworkCore
{
    public static class LIMSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<LIMSDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString, b => b.UseRowNumberForPaging());
        }

        public static void Configure(DbContextOptionsBuilder<LIMSDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection, b => b.UseRowNumberForPaging());
        }
    }
}
