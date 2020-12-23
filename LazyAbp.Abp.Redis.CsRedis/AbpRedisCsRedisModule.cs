using LazyAbp.Abp.Redis.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.Modularity;

namespace LazyAbp.Abp.Redis.CsRedis
{
    [DependsOn(
        typeof(AbpRedisAbstractionsModule)
        )]
    public class AbpRedisCsRedisModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.TryAddSingleton(typeof(IRedisServiceResolver), typeof(RedisServiceResolver));
            // 这里不要注册为单例，否则无法热更新
            context.Services.TryAddTransient<IRedisService>(serviceProvider =>
            {
                var redisServiceResolver = serviceProvider.GetService(typeof(IRedisServiceResolver)) as IRedisServiceResolver;
                return redisServiceResolver.Default();
            });
        }
    }
}
