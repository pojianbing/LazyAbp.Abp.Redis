using sample.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace sample.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(sampleEntityFrameworkCoreDbMigrationsModule),
        typeof(sampleApplicationContractsModule)
        )]
    public class sampleDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
        }
    }
}
