using System;
using System.Collections.Generic;
using System.Text;

namespace LazyAbp.Abp.Redis.Abstractions
{
    /// <summary>
    /// 服务解析
    /// </summary>
    public interface IRedisServiceResolver
    {
        /// <summary>
        /// 默认(即配置的第一个)
        /// </summary>
        /// <returns></returns>
        IRedisService Default();
        /// <summary>
        /// 根据名称解析
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IRedisService Resolve(string name);
    }
}
