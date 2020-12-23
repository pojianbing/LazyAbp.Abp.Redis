# LazyAbp.Abp.Redis
abp vnext CSRedisCore模块

## 安装
1. 安装NuGet packages
     * LazyAbp.Abp.Redis.CsRedis

2. 添加模块依赖 DependsOn(typeof(AbpRedisCsRedisModule))

3. 配置option
   ```
    /// <summary>
    /// 配置csrediscore
    /// </summary>
    /// <param name="context"></param>
    /// <param name="configuration"></param>
    /// <param name="hostingEnvironment"></param>
    private void ConfigureCsRedis(
        ServiceConfigurationContext context,
        IConfiguration configuration)
    {
        context.Services.Configure<RedisOptions>(configuration.GetSection("CsRedis"));
    }
   ```
   
4. appsettings.json
   ```
    "CsRedis": {
      "Clients": [
        {
          "Name": "0",
          "ConnectionStrings": [ "127.0.0.1,password=123456,defaultdatabase=1" ]
        },
        {
          "Name": "1",
          "ConnectionStrings": [ "127.0.0.1,password=123456,defaultdatabase=2" ]
        }
      ]
    }
   ```

## 使用
   ```
     public class IndexModel : samplePageModel
     {
         public IndexModel(IRedisService redisService, IRedisServiceResolver redisServiceResolver)
         {
             var redisService1 = redisService;
             var redisService2 = redisServiceResolver.Default();
             var redisService3 = redisServiceResolver.Resolve("0");
             var redisService4 = redisServiceResolver.Resolve("1");

             var isSame1 = redisService1 == redisService2; // 同一实例
             var isSame2 = redisService2 == redisService3; // 同一实例
             var isSame3 = redisService3 == redisService4; // 不同实例

             redisService4.Set("key", "value");
             var value = redisService4.Get("key");
         }

         public void OnGet()
         {

         }
     }
   ```
