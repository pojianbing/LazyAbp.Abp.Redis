using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace sample.EntityFrameworkCore
{
    [DependsOn(
        typeof(sampleEntityFrameworkCoreModule)
        )]
    public class sampleEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<sampleMigrationsDbContext>();
        }
    }
}
