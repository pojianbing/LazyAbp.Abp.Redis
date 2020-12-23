using LazyAbp.Abp.Redis.Abstractions;

namespace sample.Web.Pages
{
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
}