using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace sample.Data
{
    /* This is used if database provider does't define
     * IsampleDbSchemaMigrator implementation.
     */
    public class NullsampleDbSchemaMigrator : IsampleDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}