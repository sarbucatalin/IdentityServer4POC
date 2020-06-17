using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Reflection;

namespace IdentityServerPOC.Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
    {
        AppIdentityDbContext IDesignTimeDbContextFactory<AppIdentityDbContext>.CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppIdentityDbContext>();
            builder.UseSqlServer(ConfigurationUtils.GetConnectionString("Default"),
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContextFactory).GetTypeInfo().Assembly.GetName().Name));
            return new AppIdentityDbContext(builder.Options);
        }
    }
}
