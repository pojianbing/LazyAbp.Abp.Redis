using Microsoft.EntityFrameworkCore;
using Volo.Abp;

namespace sample.EntityFrameworkCore
{
    public static class sampleDbContextModelCreatingExtensions
    {
        public static void Configuresample(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            //builder.Entity<YourEntity>(b =>
            //{
            //    b.ToTable(sampleConsts.DbTablePrefix + "YourEntities", sampleConsts.DbSchema);
            //    b.ConfigureByConvention(); //auto configure for the base class props
            //    //...
            //});
        }
    }
}