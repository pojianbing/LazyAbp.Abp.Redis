using System.Threading.Tasks;

namespace sample.Data
{
    public interface IsampleDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
