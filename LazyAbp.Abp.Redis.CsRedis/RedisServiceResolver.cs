using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using CSRedis;
using LazyAbp.Abp.Redis.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace LazyAbp.Abp.Redis.CsRedis
{
    /// <summary>
    /// 服务解析
    /// </summary>
    public class RedisServiceResolver : IRedisServiceResolver
    {
        private readonly IOptionsMonitor<RedisOptions> _optionsMonitor;

        private ConcurrentDictionary<string, IRedisService> _redisMap;
        private BlockingCollection<IRedisService> _redis;

        public RedisServiceResolver(IOptionsMonitor<RedisOptions> optionsMonitor, IServiceProvider serviceCollection)
        {
            optionsMonitor.OnChange(option =>
            {
                option.Check();
                
                // 支持热更新，需要重新
                // 释放已有连接(暂时没找到对应的方法)，这里可能会有问题
                // 创建新连接
                this.initialRedisServices();
            });
            this._optionsMonitor = optionsMonitor;
            this._optionsMonitor.CurrentValue.Check();

            this.initialRedisServices();
        }

        private RedisService createRedisService(RedisClientOptions e)
        {
            CSRedisClient redisClient;
            if (e.ConnectionStrings.Length == 1)
            {
                // 单机模式
                redisClient = new CSRedisClient(e.ConnectionStrings[0]);
            }
            else
            {
                // 集群模式
                redisClient = new CSRedisClient(NodeRule: null, connectionStrings: e.ConnectionStrings);
            }
            return new RedisService(redisClient);
        }

        private void initialRedisServices()
        {
            _redisMap = new ConcurrentDictionary<string, IRedisService>();
            _redis = new BlockingCollection<IRedisService>();

            this._optionsMonitor.CurrentValue.Clients.ForEach(e =>
            {
                var redisService = createRedisService(e);
                _redisMap.TryAdd(e.Name, redisService);
                _redis.Add(redisService);
            });
        }

        public IRedisService Default()
        {
            return _redis.First();
        }

        public IRedisService Resolve(string name)
        {
            if (_redisMap.ContainsKey(name))
            {
                return _redisMap[name];
            }
            else
            {
                throw new Exception("未找到客户端");
            }
        }
    }
}
