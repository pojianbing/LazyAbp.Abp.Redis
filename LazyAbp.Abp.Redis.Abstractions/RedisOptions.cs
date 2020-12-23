using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazyAbp.Abp.Redis.Abstractions
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisOptions
    {
        /// <summary>
        /// 客户端
        /// </summary>
        public List<RedisClientOptions> Clients { get; set; }
    }

    /// <summary>
    /// 客户端选项
    /// </summary>
    public class RedisClientOptions
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Redis连接字符串，长度等于1为单机模式，长度大于0为集群模式
        /// </summary>
        public string[] ConnectionStrings { get; set; }
    }

    public static class RedisOptionsExtension
    {
        public static void Check(this RedisOptions options)
        {
            if (options == null ||
                options.Clients == null ||
                options.Clients.Count == 0)
            {
                throw new RedisConfigException("客户端未配置");
            }

            if (options.Clients.Count > 1)
            {
                var existEmptyName = options.Clients.Count(e => string.IsNullOrWhiteSpace(e.Name)) > 0;
                if (existEmptyName)
                {
                    throw new RedisConfigException("客户端超过一个时，名称必须配置");
                }

                var distinctNameCount = options.Clients.Select(e => e.Name).Distinct().Count();
                var existDuplicateName = distinctNameCount != options.Clients.Count;
                if (existDuplicateName)
                {
                    throw new RedisConfigException("客户端超过一个时，名称不允许重名");
                }
            }
        }
    }
}
