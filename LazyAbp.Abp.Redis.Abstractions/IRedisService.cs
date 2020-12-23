using CSRedis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LazyAbp.Abp.Redis.Abstractions
{
    /// <summary>
    /// Redis服务接口
    /// </summary>
    public interface IRedisService
    {
        /// <summary>
        /// 缓存壳
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="timeoutSeconds">缓存秒数</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        T CacheShell<T>(string key, int timeoutSeconds, Func<T> getData);
        /// <summary>
        /// 缓存壳(哈希表)
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <param name="timeoutSeconds">缓存秒数</param>
        /// <param name="getData">获取源数据的函数</param>
        /// <returns></returns>
        T CacheShell<T>(string key, string field, int timeoutSeconds, Func<T> getData);
        /// <summary>
        /// 缓存壳(哈希表)，将 fields 每个元素存储到单独的缓存片，实现最大化复用
        /// </summary>
        /// <typeparam name="T">缓存类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="fields">字段</param>
        /// <param name="timeoutSeconds">缓存秒数</param>
        /// <param name="getData">获取源数据的函数，输入参数是没有缓存的 fields，返回值应该是 (field, value)[]</param>
        /// <returns></returns>
        (string key, T value)[] CacheShell<T>(string key, string[] fields, int timeoutSeconds, Func<string[], (string, T)[]> getData);
        /// <summary>
        /// 创建管道传输
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        object[] StartPipe(Action<CSRedisClientPipe<string>> handler);
        /// <summary>
        /// 创建管道传输，打包提交如：RedisHelper.StartPipe().Set("a", "1").HSet("b", "f", "2").EndPipe();
        /// </summary>
        /// <returns></returns>
        CSRedisClientPipe<string> StartPipe();
        /// <summary>
        /// 在指定分区节点上，执行服务器命令
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns></returns>
        CSRedisClient.NodeServerManagerProvider NodeServerManager(string node);
        /// <summary>
        /// 打印字符串
        /// </summary>
        /// <param name="nodeKey">分区key</param>
        /// <param name="message">消息</param>
        /// <returns></returns>
        string Echo(string nodeKey, string message);
        /// <summary>
        /// 打印字符串
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        string Echo(string message);
        /// <summary>
        /// 查看服务是否运行
        /// </summary>
        /// <param name="nodeKey">分区key</param>
        /// <returns></returns>
        bool Ping(string nodeKey);
        /// <summary>
        /// 查看服务是否运行
        /// </summary>
        /// <returns></returns>
        bool Ping();
        /// <summary>
        /// 在所有分区节点上，执行服务器命令
        /// </summary>
        CSRedisClient.NodesServerManagerProvider NodesServerManager { get; }
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <param name="key">用于定位分区节点，不含prefix前辍</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        object Eval(string script, string key, params object[] args);
        /// <summary>
        /// 执行脚本
        /// </summary>
        /// <param name="sha1">脚本缓存的sha1</param>
        /// <param name="key">用于定位分区节点，不含prefix前辍</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        object EvalSHA(string sha1, string key, params object[] args);
        /// <summary>
        /// 校验所有分区节点中，脚本是否已经缓存。任何分区节点未缓存sha1，都返回false。
        /// </summary>
        /// <param name="sha1">脚本缓存的sha1</param>
        /// <returns></returns>
        bool[] ScriptExists(params string[] sha1);
        /// <summary>
        /// 清除所有分区节点中，所有 Lua 脚本缓存
        /// </summary>
        void ScriptFlush();
        /// <summary>
        /// 杀死所有分区节点中，当前正在运行的 Lua 脚本
        /// </summary>
        void ScriptKill();
        /// <summary>
        /// 在所有分区节点中，缓存脚本后返回 sha1（同样的脚本在任何服务器，缓存后的 sha1 都是相同的）
        /// </summary>
        /// <param name="script">Lua 脚本</param>
        /// <returns></returns>
        string ScriptLoad(string script);
        /// <summary>
        /// 用于将信息发送到指定分区节点的频道，最终消息发布格式：1|message
        /// </summary>
        /// <param name="channel">频道名</param>
        /// <param name="message">消息文本</param>
        /// <returns></returns>
        long Publish(string channel, string message);
        /// <summary>
        /// 用于将信息发送到指定分区节点的频道，与 Publish 方法不同，不返回消息id头，即 1|
        /// </summary>
        /// <param name="channel">频道名</param>
        /// <param name="message">消息文本</param>
        /// <returns></returns>
        long PublishNoneMessageId(string channel, string message);
        /// <summary>
        /// 查看所有订阅频道
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        string[] PubSubChannels(string pattern);
        /// <summary>
        /// 查看所有模糊订阅端的数量<para></para>
        /// 注意：分区模式下，其他客户端的订阅可能不会返回
        /// </summary>
        /// <returns></returns>
        long PubSubNumPat();
        /// <summary>
        /// 查看所有订阅端的数量<para></para>
        /// 注意：分区模式下，其他客户端的订阅可能不会返回
        /// </summary>
        /// <param name="channels">频道</param>
        /// <returns></returns>
        Dictionary<string, long> PubSubNumSub(params string[] channels);
        /// <summary>
        /// 订阅，根据分区规则返回SubscribeObject，Subscribe(("chan1", msg => Console.WriteLine(msg.Body)), ("chan2", msg => Console.WriteLine(msg.Body)))
        /// </summary>
        /// <param name="channels">频道和接收器</param>
        /// <returns>返回可停止订阅的对象</returns>
        CSRedisClient.SubscribeObject Subscribe(params (string, Action<CSRedisClient.SubscribeMessageEventArgs>)[] channels);
        /// <summary>
        /// 模糊订阅，订阅所有分区节点(同条消息只处理一次），返回SubscribeObject，PSubscribe(new [] { "chan1*", "chan2*" }, msg => Console.WriteLine(msg.Body))
        /// </summary>
        /// <param name="channelPatterns">模糊频道</param>
        /// <param name="pmessage">接收器</param>
        /// <returns>返回可停止模糊订阅的对象</returns>
        CSRedisClient.PSubscribeObject PSubscribe(string[] channelPatterns, Action<CSRedisClient.PSubscribePMessageEventArgs> pmessage);
        /// <summary>
        /// 使用lpush + blpop订阅端（多端非争抢模式），都可以收到消息
        /// </summary>
        /// <param name="listKey">list key（不含prefix前辍）</param>
        /// <param name="clientId">订阅端标识，若重复则争抢，若唯一必然收到消息</param>
        /// <param name="onMessage">接收消息委托</param>
        /// <returns></returns>
        CSRedisClient.SubscribeListBroadcastObject SubscribeListBroadcast(string listKey, string clientId, Action<string> onMessage);
        /// <summary>
        /// 使用lpush + blpop订阅端（多端争抢模式），只有一端收到消息
        /// </summary>
        /// <param name="listKey">list key（不含prefix前辍）</param>
        /// <param name="onMessage">接收消息委托</param>
        /// <returns></returns>
        CSRedisClient.SubscribeListObject SubscribeList(string listKey, Action<string> onMessage);
        /// <summary>
        /// 使用lpush + blpop订阅端（多端争抢模式），只有一端收到消息
        /// </summary>
        /// <param name="listKeys">支持多个 key（不含prefix前辍）</param>
        /// <param name="onMessage">接收消息委托，参数1：key；参数2：消息体</param>
        /// <returns></returns>
        CSRedisClient.SubscribeListObject SubscribeList(string[] listKeys, Action<string, string> onMessage);
        /// <summary>
        /// 添加指定元素到 HyperLogLog
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="elements">元素</param>
        /// <returns></returns>
        bool PfAdd<T>(string key, params T[] elements);
        /// <summary>
        /// 返回给定 HyperLogLog 的基数估算值<para></para>
        /// 注意：分区模式下，若keys分散在多个分区节点时，将报错
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        long PfCount(params string[] keys);
        /// <summary>
        /// 将多个 HyperLogLog 合并为一个 HyperLogLog<para></para>
        /// 注意：分区模式下，若keys分散在多个分区节点时，将报错
        /// </summary>
        /// <param name="destKey">新的 HyperLogLog，不含prefix前辍</param>
        /// <param name="sourceKeys">源 HyperLogLog，不含prefix前辍</param>
        /// <returns></returns>
        bool PfMerge(string destKey, params string[] sourceKeys);
        /// <summary>
        /// 向有序集合添加一个或多个成员，或者更新已存在成员的分数
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="scoreMembers">一个或多个成员分数</param>
        /// <returns></returns>
        long ZAdd(string key, params (decimal, object)[] scoreMembers);
        /// <summary>
        /// 获取有序集合的成员数量
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long ZCard(string key);
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数量
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <returns></returns>
        long ZCount(string key, decimal min, decimal max);
        /// <summary>
        /// 计算在有序集合中指定区间分数的成员数量
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <returns></returns>
        long ZCount(string key, string min, string max);
        /// <summary>
        /// 有序集合中对指定成员的分数加上增量 increment
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="increment">增量值(默认=1)</param>
        /// <returns></returns>
        decimal ZIncrBy(string key, string member, decimal increment = 1);
        /// <summary>
        /// 计算给定的一个或多个有序集的交集，将结果集存储在新的有序集合 destination 中
        /// </summary>
        /// <param name="destination">新的有序集合，不含prefix前辍</param>
        /// <param name="weights">使用 WEIGHTS 选项，你可以为 每个 给定有序集 分别 指定一个乘法因子。如果没有指定 WEIGHTS 选项，乘法因子默认设置为 1 。</param>
        /// <param name="aggregate">Sum | Min | Max</param>
        /// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
        /// <returns></returns>
        long ZInterStore(string destination, decimal[] weights, RedisAggregate aggregate, params string[] keys);
        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        string[] ZRange(string key, long start, long stop);
        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        T[] ZRange<T>(string key, long start, long stop);
        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员和分数
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRangeWithScores(string key, long start, long stop);
        /// <summary>
        /// 通过索引区间返回有序集合成指定区间内的成员和分数
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRangeWithScores<T>(string key, long start, long stop);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        string[] ZRangeByScore(string key, decimal min, decimal max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        T[] ZRangeByScore<T>(string key, decimal min, decimal max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        string[] ZRangeByScore(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        T[] ZRangeByScore<T>(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRangeByScoreWithScores(string key, decimal min, decimal max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRangeByScoreWithScores<T>(string key, decimal min, decimal max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRangeByScoreWithScores(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 通过分数返回有序集合指定区间内的成员和分数
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRangeByScoreWithScores<T>(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集合中指定成员的索引
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        long? ZRank(string key, object member);
        /// <summary>
        /// 移除有序集合中的一个或多个成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">一个或多个成员</param>
        /// <returns></returns>
        long ZRem<T>(string key, params T[] member);
        /// <summary>
        /// 移除有序集合中给定的排名区间的所有成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        long ZRemRangeByRank(string key, long start, long stop);
        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <returns></returns>
        long ZRemRangeByScore(string key, decimal min, decimal max);
        /// <summary>
        /// 移除有序集合中给定的分数区间的所有成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <returns></returns>
        long ZRemRangeByScore(string key, string min, string max);
        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引，分数从高到底
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        string[] ZRevRange(string key, long start, long stop);
        /// <summary>
        /// 返回有序集中指定区间内的成员，通过索引，分数从高到底
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        T[] ZRevRange<T>(string key, long start, long stop);
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引，分数从高到底
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRevRangeWithScores(string key, long start, long stop);
        /// <summary>
        /// 返回有序集中指定区间内的成员和分数，通过索引，分数从高到底
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRevRangeWithScores<T>(string key, long start, long stop);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        string[] ZRevRangeByScore(string key, decimal max, decimal min, long? limit = null, long? offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        T[] ZRevRangeByScore<T>(string key, decimal max, decimal min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        string[] ZRevRangeByScore(string key, string max, string min, long? limit = null, long? offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        T[] ZRevRangeByScore<T>(string key, string max, string min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRevRangeByScoreWithScores(string key, decimal max, decimal min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 decimal.MaxValue 10</param>
        /// <param name="min">分数最小值 decimal.MinValue 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRevRangeByScoreWithScores<T>(string key, decimal max, decimal min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (string member, decimal score)[] ZRevRangeByScoreWithScores(string key, string max, string min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集中指定分数区间内的成员和分数，分数从高到低排序
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="max">分数最大值 +inf (10 10</param>
        /// <param name="min">分数最小值 -inf (1 1</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        (T member, decimal score)[] ZRevRangeByScoreWithScores<T>(string key, string max, string min, long? limit = null, long offset = 0);
        /// <summary>
        /// 返回有序集合中指定成员的排名，有序集成员按分数值递减(从大到小)排序
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        long? ZRevRank(string key, object member);
        /// <summary>
        /// 返回有序集中，成员的分数值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        decimal? ZScore(string key, object member);
        /// <summary>
        /// 计算给定的一个或多个有序集的并集，将结果集存储在新的有序集合 destination 中
        /// </summary>
        /// <param name="destination">新的有序集合，不含prefix前辍</param>
        /// <param name="weights">使用 WEIGHTS 选项，你可以为 每个 给定有序集 分别 指定一个乘法因子。如果没有指定 WEIGHTS 选项，乘法因子默认设置为 1 。</param>
        /// <param name="aggregate">Sum | Min | Max</param>
        /// <param name="keys">一个或多个有序集合，不含prefix前辍</param>
        /// <returns></returns>
        long ZUnionStore(string destination, decimal[] weights, RedisAggregate aggregate, params string[] keys);
        /// <summary>
        /// 迭代有序集合中的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<(string member, decimal score)> ZScan(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 迭代有序集合中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<(T member, decimal score)> ZScan<T>(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        string[] ZRangeByLex(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="limit">返回多少成员</param>
        /// <param name="offset">返回条件偏移位置</param>
        /// <returns></returns>
        T[] ZRangeByLex<T>(string key, string min, string max, long? limit = null, long offset = 0);
        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <returns></returns>
        long ZRemRangeByLex(string key, string min, string max);
        /// <summary>
        /// 当有序集合的所有成员都具有相同的分值时，有序集合的元素会根据成员的字典序来进行排序，这个命令可以返回给定的有序集合键 key 中，值介于 min 和 max 之间的成员。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="min">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <param name="max">'(' 表示包含在范围，'[' 表示不包含在范围，'+' 正无穷大，'-' 负无限。 ZRANGEBYLEX zset - + ，命令将返回有序集合中的所有元素</param>
        /// <returns></returns>
        long ZLexCount(string key, string min, string max);
        /// <summary>
        /// [redis-server 5.0.0] 删除并返回有序集合key中的最多count个具有最高得分的成员。如未指定，count的默认值为1。指定一个大于有序集合的基数的count不会产生错误。 当返回多个元素时候，得分最高的元素将是第一个元素，然后是分数较低的元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        (string member, decimal score)[] ZPopMax(string key, long count);
        /// <summary>
        /// [redis-server 5.0.0] 删除并返回有序集合key中的最多count个具有最高得分的成员。如未指定，count的默认值为1。指定一个大于有序集合的基数的count不会产生错误。 当返回多个元素时候，得分最高的元素将是第一个元素，然后是分数较低的元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        (T member, decimal score)[] ZPopMax<T>(string key, long count);
        /// <summary>
        /// [redis-server 5.0.0] 删除并返回有序集合key中的最多count个具有最低得分的成员。如未指定，count的默认值为1。指定一个大于有序集合的基数的count不会产生错误。 当返回多个元素时候，得分最低的元素将是第一个元素，然后是分数较高的元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        (string member, decimal score)[] ZPopMin(string key, long count);
        /// <summary>
        /// [redis-server 5.0.0] 删除并返回有序集合key中的最多count个具有最低得分的成员。如未指定，count的默认值为1。指定一个大于有序集合的基数的count不会产生错误。 当返回多个元素时候，得分最低的元素将是第一个元素，然后是分数较高的元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        (T member, decimal score)[] ZPopMin<T>(string key, long count);
        /// <summary>
        /// 向集合添加一个或多个成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="members">一个或多个成员</param>
        /// <returns></returns>
        long SAdd<T>(string key, params T[] members);
        /// <summary>
        /// 获取集合的成员数
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long SCard(string key);
        /// <summary>
        /// 返回给定所有集合的差集
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        string[] SDiff(params string[] keys);
        /// <summary>
        /// 返回给定所有集合的差集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        T[] SDiff<T>(params string[] keys);
        /// <summary>
        /// 返回给定所有集合的差集并存储在 destination 中
        /// </summary>
        /// <param name="destination">新的无序集合，不含prefix前辍</param>
        /// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
        /// <returns></returns>
        long SDiffStore(string destination, params string[] keys);
        /// <summary>
        /// 返回给定所有集合的交集
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        string[] SInter(params string[] keys);
        /// <summary>
        /// 返回给定所有集合的交集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        T[] SInter<T>(params string[] keys);
        /// <summary>
        /// 返回给定所有集合的交集并存储在 destination 中
        /// </summary>
        /// <param name="destination">新的无序集合，不含prefix前辍</param>
        /// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
        /// <returns></returns>
        long SInterStore(string destination, params string[] keys);
        /// <summary>
        /// 判断 member 元素是否是集合 key 的成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        bool SIsMember(string key, object member);
        /// <summary>
        /// 返回集合中的所有成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string[] SMembers(string key);
        /// <summary>
        /// 返回集合中的所有成员
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T[] SMembers<T>(string key);
        /// <summary>
        /// 将 member 元素从 source 集合移动到 destination 集合
        /// </summary>
        /// <param name="source">无序集合key，不含prefix前辍</param>
        /// <param name="destination">目标无序集合key，不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <returns></returns>
        bool SMove(string source, string destination, object member);
        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string SPop(string key);
        /// <summary>
        /// 移除并返回集合中的一个随机元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T SPop<T>(string key);
        /// <summary>
        /// [redis-server 3.2] 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">移除并返回的个数</param>
        /// <returns></returns>
        string[] SPop(string key, long count);
        /// <summary>
        /// [redis-server 3.2] 移除并返回集合中的一个或多个随机元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">移除并返回的个数</param>
        /// <returns></returns>
        T[] SPop<T>(string key, long count);
        /// <summary>
        /// 返回集合中的一个随机元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string SRandMember(string key);
        /// <summary>
        /// 返回集合中的一个随机元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T SRandMember<T>(string key);
        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        string[] SRandMembers(string key, int count = 1);
        /// <summary>
        /// 返回集合中一个或多个随机数的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">返回个数</param>
        /// <returns></returns>
        T[] SRandMembers<T>(string key, int count = 1);
        /// <summary>
        /// 移除集合中一个或多个成员
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="members">一个或多个成员</param>
        /// <returns></returns>
        long SRem<T>(string key, params T[] members);
        /// <summary>
        /// 返回所有给定集合的并集
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        string[] SUnion(params string[] keys);
        /// <summary>
        /// 返回所有给定集合的并集
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        T[] SUnion<T>(params string[] keys);
        /// <summary>
        /// 所有给定集合的并集存储在 destination 集合中
        /// </summary>
        /// <param name="destination">新的无序集合，不含prefix前辍</param>
        /// <param name="keys">一个或多个无序集合，不含prefix前辍</param>
        /// <returns></returns>
        long SUnionStore(string destination, params string[] keys);
        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<string> SScan(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 迭代集合中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<T> SScan<T>(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        (string key, string value)? BLPopWithKey(int timeout, params string[] keys);
        /// <summary>
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        (string key, T value)? BLPopWithKey<T>(int timeout, params string[] keys);
        /// <summary>
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        string BLPop(int timeout, params string[] keys);
        /// <summary>
        /// 它是 LPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BLPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        T BLPop<T>(int timeout, params string[] keys);
        /// <summary>
        /// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        (string key, string value)? BRPopWithKey(int timeout, params string[] keys);
        /// <summary>
        /// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        (string key, T value)? BRPopWithKey<T>(int timeout, params string[] keys);
        /// <summary>
        /// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        string BRPop(int timeout, params string[] keys);
        /// <summary>
        /// 它是 RPOP 命令的阻塞版本，当给定列表内没有任何元素可供弹出的时候，连接将被 BRPOP 命令阻塞，直到等待超时或发现可弹出元素为止，超时返回null
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="timeout">超时(秒)</param>
        /// <param name="keys">一个或多个列表，不含prefix前辍</param>
        /// <returns></returns>
        T BRPop<T>(int timeout, params string[] keys);
        /// <summary>
        /// BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本，当给定列表 source 不为空时， BRPOPLPUSH 的表现和 RPOPLPUSH 一样。
        /// 当列表 source 为空时， BRPOPLPUSH 命令将阻塞连接，直到等待超时，或有另一个客户端对 source 执行 LPUSH 或 RPUSH 命令为止。
        /// </summary>
        /// <param name="source">源key，不含prefix前辍</param>
        /// <param name="destination">目标key，不含prefix前辍</param>
        /// <param name="timeout">超时(秒)</param>
        /// <returns></returns>
        string BRPopLPush(string source, string destination, int timeout);
        /// <summary>
        /// BRPOPLPUSH 是 RPOPLPUSH 的阻塞版本，当给定列表 source 不为空时， BRPOPLPUSH 的表现和 RPOPLPUSH 一样。
        /// 当列表 source 为空时， BRPOPLPUSH 命令将阻塞连接，直到等待超时，或有另一个客户端对 source 执行 LPUSH 或 RPUSH 命令为止。
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="source">源key，不含prefix前辍</param>
        /// <param name="destination">目标key，不含prefix前辍</param>
        /// <param name="timeout">超时(秒)</param>
        /// <returns></returns>
        T BRPopLPush<T>(string source, string destination, int timeout);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        string LIndex(string key, long index);
        /// <summary>
        /// 通过索引获取列表中的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="index">索引</param>
        /// <returns></returns>
        T LIndex<T>(string key, long index);
        /// <summary>
        /// 在列表中的元素前面插入元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="pivot">列表的元素</param>
        /// <param name="value">新元素</param>
        /// <returns></returns>
        long LInsertBefore(string key, object pivot, object value);
        /// <summary>
        /// 在列表中的元素后面插入元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="pivot">列表的元素</param>
        /// <param name="value">新元素</param>
        /// <returns></returns>
        long LInsertAfter(string key, object pivot, object value);
        /// <summary>
        /// 获取列表长度
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long LLen(string key);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string LPop(string key);
        /// <summary>
        /// 移出并获取列表的第一个元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T LPop<T>(string key);
        /// <summary>
        /// 将一个或多个值插入到列表头部
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 LPUSH 命令后，列表的长度</returns>
        long LPush<T>(string key, params T[] value);
        /// <summary>
        /// 将一个值插入到已存在的列表头部
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <returns>执行 LPUSHX 命令后，列表的长度。</returns>
        long LPushX(string key, object value);
        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        string[] LRange(string key, long start, long stop);
        /// <summary>
        /// 获取列表指定范围内的元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        T[] LRange<T>(string key, long start, long stop);
        /// <summary>
        /// 根据参数 count 的值，移除列表中与参数 value 相等的元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="count">移除的数量，大于0时从表头删除数量count，小于0时从表尾删除数量-count，等于0移除所有</param>
        /// <param name="value">元素</param>
        /// <returns></returns>
        long LRem(string key, long count, object value);
        /// <summary>
        /// 通过索引设置列表元素的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="index">索引</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool LSet(string key, long index, object value);
        /// <summary>
        /// 对一个列表进行修剪，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="stop">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        bool LTrim(string key, long start, long stop);
        /// <summary>
        /// 移除并获取列表最后一个元素
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string RPop(string key);
        /// <summary>
        /// 移除并获取列表最后一个元素
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T RPop<T>(string key);
        /// <summary>
        /// 将列表 source 中的最后一个元素(尾元素)弹出，并返回给客户端。
        /// 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。
        /// </summary>
        /// <param name="source">源key，不含prefix前辍</param>
        /// <param name="destination">目标key，不含prefix前辍</param>
        /// <returns></returns>
        string RPopLPush(string source, string destination);
        /// <summary>
        /// 将列表 source 中的最后一个元素(尾元素)弹出，并返回给客户端。
        /// 将 source 弹出的元素插入到列表 destination ，作为 destination 列表的的头元素。
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="source">源key，不含prefix前辍</param>
        /// <param name="destination">目标key，不含prefix前辍</param>
        /// <returns></returns>
        T RPopLPush<T>(string source, string destination);
        /// <summary>
        /// 在列表中添加一个或多个值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 RPUSH 命令后，列表的长度</returns>
        long RPush<T>(string key, params T[] value);
        /// <summary>
        /// 为已存在的列表添加值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">一个或多个值</param>
        /// <returns>执行 RPUSHX 命令后，列表的长度</returns>
        long RPushX(string key, object value);
        /// <summary>
        /// 删除一个或多个哈希表字段
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        long HDel(string key, params string[] fields);
        /// <summary>
        /// 查看哈希表 key 中，指定的字段是否存在
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        bool HExists(string key, string field);
        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        string HGet(string key, string field);
        /// <summary>
        /// 获取存储在哈希表中指定字段的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <returns></returns>
        T HGet<T>(string key, string field);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        Dictionary<string, string> HGetAll(string key);
        /// <summary>
        /// 获取在哈希表中指定 key 的所有字段和值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        Dictionary<string, T> HGetAll<T>(string key);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        long HIncrBy(string key, string field, long value = 1);
        /// <summary>
        /// 为哈希表 key 中的指定字段的整数值加上增量 increment
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        decimal HIncrByFloat(string key, string field, decimal value = 1);
        /// <summary>
        /// 获取所有哈希表中的字段
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string[] HKeys(string key);
        /// <summary>
        /// 获取哈希表中字段的数量
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long HLen(string key);
        /// <summary>
        /// 获取存储在哈希表中多个字段的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="fields">字段</param>
        /// <returns></returns>
        string[] HMGet(string key, params string[] fields);
        /// <summary>
        /// 获取存储在哈希表中多个字段的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="fields">一个或多个字段</param>
        /// <returns></returns>
        T[] HMGet<T>(string key, params string[] fields);
        /// <summary>
        /// 同时将多个 field-value (域-值)对设置到哈希表 key 中
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        /// <returns></returns>
        bool HMSet(string key, params object[] keyValues);
        /// <summary>
        /// 将哈希表 key 中的字段 field 的值设为 value
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <param name="value">值</param>
        /// <returns>如果字段是哈希表中的一个新建字段，并且值设置成功，返回true。如果哈希表中域字段已经存在且旧值已被新值覆盖，返回false。</returns>
        bool HSet(string key, string field, object value);
        /// <summary>
        /// 只有在字段 field 不存在时，设置哈希表字段的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="field">字段</param>
        /// <param name="value">值(string 或 byte[])</param>
        /// <returns></returns>
        bool HSetNx(string key, string field, object value);
        /// <summary>
        /// 获取哈希表中所有值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string[] HVals(string key);
        /// <summary>
        /// 获取哈希表中所有值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T[] HVals<T>(string key);
        /// <summary>
        /// 迭代哈希表中的键值对
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<(string field, string value)> HScan(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 迭代哈希表中的键值对
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<(string field, T value)> HScan<T>(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 如果 key 已经存在并且是一个字符串， APPEND 命令将指定的 value 追加到该 key 原来值（value）的末尾
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">字符串</param>
        /// <returns>追加指定值之后， key 中字符串的长度</returns>
        long Append(string key, object value);
        /// <summary>
        /// 计算给定位置被设置为 1 的比特位的数量
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置</param>
        /// <param name="end">结束位置</param>
        /// <returns></returns>
        long BitCount(string key, long start, long end);
        /// <summary>
        /// 对一个或多个保存二进制位的字符串 key 进行位元操作，并将结果保存到 destkey 上
        /// </summary>
        /// <param name="op">And | Or | XOr | Not</param>
        /// <param name="destKey">不含prefix前辍</param>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns>保存到 destkey 的长度，和输入 key 中最长的长度相等</returns>
        long BitOp(RedisBitOp op, string destKey, params string[] keys);
        /// <summary>
        /// 对 key 所储存的值，查找范围内第一个被设置为1或者0的bit位
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="bit">查找值</param>
        /// <param name="start">开始位置，-1是最后一个，-2是倒数第二个</param>
        /// <param name="end">结果位置，-1是最后一个，-2是倒数第二个</param>
        /// <returns>返回范围内第一个被设置为1或者0的bit位</returns>
        long BitPos(string key, bool bit, long? start = null, long? end = null);
        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string Get(string key);
        /// <summary>
        /// 获取指定 key 的值
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        T Get<T>(string key);
        /// <summary>
        /// 获取指定 key 的值（适用大对象返回）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="destination">读取后写入目标流中</param>
        /// <param name="bufferSize">读取缓冲区</param>
        void Get(string key, Stream destination, int bufferSize = 1024);
        /// <summary>
        /// 对 key 所储存的值，获取指定偏移量上的位(bit)
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="offset">偏移量</param>
        /// <returns></returns>
        bool GetBit(string key, uint offset);
        /// <summary>
        /// 返回 key 中字符串值的子字符
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="end">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        string GetRange(string key, long start, long end);
        /// <summary>
        /// 返回 key 中字符串值的子字符
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="start">开始位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <param name="end">结束位置，0表示第一个元素，-1表示最后一个元素</param>
        /// <returns></returns>
        T GetRange<T>(string key, long start, long end);
        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        string GetSet(string key, object value);
        /// <summary>
        /// 将给定 key 的值设为 value ，并返回 key 的旧值(old value)
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        T GetSet<T>(string key, object value);
        /// <summary>
        /// 将 key 所储存的值加上给定的增量值（increment）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        long IncrBy(string key, long value = 1);
        /// <summary>
        /// 将 key 所储存的值加上给定的浮点增量值（increment）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">增量值(默认=1)</param>
        /// <returns></returns>
        decimal IncrByFloat(string key, decimal value = 1);
        /// <summary>
        /// 获取多个指定 key 的值(数组)
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        string[] MGet(params string[] keys);
        /// <summary>
        /// 获取多个指定 key 的值(数组)
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        T[] MGet<T>(params string[] keys);
        /// <summary>
        /// 同时设置一个或多个 key-value 对
        /// </summary>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        /// <returns></returns>
        bool MSet(params object[] keyValues);
        /// <summary>
        /// 同时设置一个或多个 key-value 对，当且仅当所有给定 key 都不存在
        /// </summary>
        /// <param name="keyValues">key1 value1 [key2 value2]</param>
        /// <returns></returns>
        bool MSetNx(params object[] keyValues);
        /// <summary>
        /// 设置指定 key 的值，所有写入参数object都支持string | byte[] | 数值 | 对象
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <param name="expireSeconds">过期(秒单位)</param>
        /// <param name="exists">Nx, Xx</param>
        /// <returns></returns>
        bool Set(string key, object value, int expireSeconds = -1, RedisExistence? exists = null);
        bool Set(string key, object value, TimeSpan expire, RedisExistence? exists = null);
        /// <summary>
        /// 对 key 所储存的字符串值，设置或清除指定偏移量上的位(bit)
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool SetBit(string key, uint offset, bool value);
        /// <summary>
        /// 只有在 key 不存在时设置 key 的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool SetNx(string key, object value);
        /// <summary>
        /// 用 value 参数覆写给定 key 所储存的字符串值，从偏移量 offset 开始
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="offset">偏移量</param>
        /// <param name="value">值</param>
        /// <returns>被修改后的字符串长度</returns>
        long SetRange(string key, uint offset, object value);
        /// <summary>
        /// 返回 key 所储存的字符串值的长度
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long StrLen(string key);
        /// <summary>
        /// 用于在 key 存在时删除 key
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long Del(params string[] key);
        /// <summary>
        /// 序列化给定 key ，并返回被序列化的值
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        byte[] Dump(string key);
        /// <summary>
        /// 检查给定 key 是否存在
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        bool Exists(string key);
        /// <summary>
        /// [redis-server 3.0] 检查给定多个 key 是否存在
        /// </summary>
        /// <param name="keys">不含prefix前辍</param>
        /// <returns></returns>
        long Exists(string[] keys);
        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="seconds">过期秒数</param>
        /// <returns></returns>
        bool Expire(string key, int seconds);
        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        bool Expire(string key, TimeSpan expire);
        /// <summary>
        /// 为给定 key 设置过期时间
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        bool ExpireAt(string key, DateTime expire);
        /// <summary>
        /// 查找所有分区节点中符合给定模式(pattern)的 key
        /// <para>Keys方法返回的keys[]包含prefix，使用前请自行处理</para>
        /// </summary>
        /// <param name="pattern">如：runoob*</param>
        /// <returns></returns>
        string[] Keys(string pattern);
        /// <summary>
        /// 将当前数据库的 key 移动到给定的数据库 db 当中
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="database">数据库</param>
        /// <returns></returns>
        bool Move(string key, int database);
        /// <summary>
        /// 该返回给定 key 锁储存的值所使用的内部表示(representation)
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        string ObjectEncoding(string key);
        /// <summary>
        /// 该返回给定 key 引用所储存的值的次数。此命令主要用于除错
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long? ObjectRefCount(string key);
        /// <summary>
        /// 返回给定 key 自储存以来的空转时间(idle， 没有被读取也没有被写入)，以秒为单位
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long? ObjectIdleTime(string key);
        /// <summary>
        /// 移除 key 的过期时间，key 将持久保持
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        bool Persist(string key);
        /// <summary>
        /// 为给定 key 设置过期时间（毫秒）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="milliseconds">过期毫秒数</param>
        /// <returns></returns>
        bool PExpire(string key, int milliseconds);
        /// <summary>
        /// 为给定 key 设置过期时间（毫秒）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        bool PExpire(string key, TimeSpan expire);
        /// <summary>
        /// 为给定 key 设置过期时间（毫秒）
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="expire">过期时间</param>
        /// <returns></returns>
        bool PExpireAt(string key, DateTime expire);
        /// <summary>
        /// 以毫秒为单位返回 key 的剩余的过期时间
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long PTtl(string key);
        /// <summary>
        /// 从所有节点中随机返回一个 key
        /// </summary>
        /// <returns>返回的 key 如果包含 prefix前辍，则会去除后返回</returns>
        string RandomKey();
        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        /// <param name="key">旧名称，不含prefix前辍</param>
        /// <param name="newKey">新名称，不含prefix前辍</param>
        /// <returns></returns>
        bool Rename(string key, string newKey);
        /// <summary>
        /// 修改 key 的名称
        /// </summary>
        /// <param name="key">旧名称，不含prefix前辍</param>
        /// <param name="newKey">新名称，不含prefix前辍</param>
        /// <returns></returns>
        bool RenameNx(string key, string newKey);
        /// <summary>
        /// 反序列化给定的序列化值，并将它和给定的 key 关联
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="serializedValue">序列化值</param>
        /// <returns></returns>
        bool Restore(string key, byte[] serializedValue);
        /// <summary>
        /// 反序列化给定的序列化值，并将它和给定的 key 关联
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="ttlMilliseconds">毫秒为单位为 key 设置生存时间</param>
        /// <param name="serializedValue">序列化值</param>
        /// <returns></returns>
        bool Restore(string key, long ttlMilliseconds, byte[] serializedValue);
        /// <summary>
        /// 返回给定列表、集合、有序集合 key 中经过排序的元素，参数资料：http://doc.redisfans.com/key/sort.html
        /// </summary>
        /// <param name="key">列表、集合、有序集合，不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <param name="offset">偏移量</param>
        /// <param name="by">排序字段</param>
        /// <param name="dir">排序方式</param>
        /// <param name="isAlpha">对字符串或数字进行排序</param>
        /// <param name="get">根据排序的结果来取出相应的键值</param>
        /// <returns></returns>
        string[] Sort(string key, long? count = null, long offset = 0, string by = null, RedisSortDir? dir = null, bool? isAlpha = null, params string[] get);
        /// <summary>
        /// 保存给定列表、集合、有序集合 key 中经过排序的元素，参数资料：http://doc.redisfans.com/key/sort.html
        /// </summary>
        /// <param name="key">列表、集合、有序集合，不含prefix前辍</param>
        /// <param name="destination">目标key，不含prefix前辍</param>
        /// <param name="count">数量</param>
        /// <param name="offset">偏移量</param>
        /// <param name="by">排序字段</param>
        /// <param name="dir">排序方式</param>
        /// <param name="isAlpha">对字符串或数字进行排序</param>
        /// <param name="get">根据排序的结果来取出相应的键值</param>
        /// <returns></returns>
        long SortAndStore(string key, string destination, long? count = null, long offset = 0, string by = null, RedisSortDir? dir = null, bool? isAlpha = null, params string[] get);
        /// <summary>
        /// 以秒为单位，返回给定 key 的剩余生存时间
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        long Ttl(string key);
        /// <summary>
        /// 返回 key 所储存的值的类型
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <returns></returns>
        KeyType Type(string key);
        /// <summary>
        /// 迭代当前数据库中的数据库键
        /// </summary>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<string> Scan(long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 迭代当前数据库中的数据库键
        /// </summary>
        /// <typeparam name="T">byte[] 或其他类型</typeparam>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="cursor">位置</param>
        /// <param name="pattern">模式</param>
        /// <param name="count">数量</param>
        /// <returns></returns>
        RedisScan<T> Scan<T>(string key, long cursor, string pattern = null, long? count = null);
        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="member">成员</param>
        /// <returns>是否成功</returns>
        bool GeoAdd(string key, decimal longitude, decimal latitude, object member);
        /// <summary>
        /// 将指定的地理空间位置（纬度、经度、成员）添加到指定的key中。这些数据将会存储到sorted set这样的目的是为了方便使用GEORADIUS或者GEORADIUSBYMEMBER命令对数据进行半径查询等操作。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="values">批量添加的值</param>
        /// <returns>添加到sorted set元素的数目，但不包括已更新score的元素。</returns>
        long GeoAdd(string key, params (decimal longitude, decimal latitude, object member)[] values);
        /// <summary>
        /// 返回两个给定位置之间的距离。如果两个位置之间的其中一个不存在， 那么命令返回空值。GEODIST 命令在计算距离时会假设地球为完美的球形， 在极限情况下， 这一假设最大会造成 0.5% 的误差。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member1">成员1</param>
        /// <param name="member2">成员2</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <returns>计算出的距离会以双精度浮点数的形式被返回。 如果给定的位置元素不存在， 那么命令返回空值。</returns>
        decimal? GeoDist(string key, object member1, object member2, GeoUnit unit = GeoUnit.m);
        /// <summary>
        /// 返回一个或多个位置元素的 Geohash 表示。通常使用表示位置的元素使用不同的技术，使用Geohash位置52点整数编码。由于编码和解码过程中所使用的初始最小和最大坐标不同，编码的编码也不同于标准。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="members">多个查询的成员</param>
        /// <returns>一个数组， 数组的每个项都是一个 geohash 。 命令返回的 geohash 的位置与用户给定的位置元素的位置一一对应。</returns>
        string[] GeoHash(string key, object[] members);
        /// <summary>
        /// 从key里返回所有给定位置元素的位置（经度和纬度）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="members">多个查询的成员</param>
        /// <returns>GEOPOS 命令返回一个数组， 数组中的每个项都由两个元素组成： 第一个元素为给定位置元素的经度， 而第二个元素则为给定位置元素的纬度。当给定的位置元素不存在时， 对应的数组项为空值。</returns>
        (decimal longitude, decimal latitude)?[] GeoPos(string key, object[] members);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        string[] GeoRadius(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        T[] GeoRadius<T>(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (string member, decimal dist)[] GeoRadiusWithDist(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (T member, decimal dist)[] GeoRadiusWithDist<T>(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (string member, decimal dist, decimal longitude, decimal latitude)[] GeoRadiusWithDistAndCoord(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的经纬度为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="longitude">经度</param>
        /// <param name="latitude">纬度</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (T member, decimal dist, decimal longitude, decimal latitude)[] GeoRadiusWithDistAndCoord<T>(string key, decimal longitude, decimal latitude, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        string[] GeoRadiusByMember(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        T[] GeoRadiusByMember<T>(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (string member, decimal dist)[] GeoRadiusByMemberWithDist(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (T member, decimal dist)[] GeoRadiusByMemberWithDist<T>(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (string member, decimal dist, decimal longitude, decimal latitude)[] GeoRadiusByMemberWithDistAndCoord(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary>
        /// 以给定的成员为中心， 返回键包含的位置元素当中， 与中心的距离不超过给定最大距离的所有位置元素（包含距离、经度、纬度）。
        /// </summary>
        /// <param name="key">不含prefix前辍</param>
        /// <param name="member">成员</param>
        /// <param name="radius">距离</param>
        /// <param name="unit">m 表示单位为米；km 表示单位为千米；mi 表示单位为英里；ft 表示单位为英尺；</param>
        /// <param name="count">虽然用户可以使用 COUNT 选项去获取前 N 个匹配元素， 但是因为命令在内部可能会需要对所有被匹配的元素进行处理， 所以在对一个非常大的区域进行搜索时， 即使只使用 COUNT 选项去获取少量元素， 命令的执行速度也可能会非常慢。 但是从另一方面来说， 使用 COUNT 选项去减少需要返回的元素数量， 对于减少带宽来说仍然是非常有用的。</param>
        /// <param name="sorting">排序</param>
        /// <returns></returns>
        (T member, decimal dist, decimal longitude, decimal latitude)[] GeoRadiusByMemberWithDistAndCoord<T>(string key, object member, decimal radius, GeoUnit unit = GeoUnit.m, long? count = null, GeoOrderBy? sorting = null);
        /// <summary> 
        /// 开启分布式锁，若超时返回null
        /// </summary>
        /// <param name="name">锁名称</param>
        /// <param name="timeoutSeconds">超时（秒）</param>
        /// <param name="autoDelay">自动延长锁超时时间，看门狗线程的超时时间为timeoutSeconds/2 ， 在看门狗线程超时时间时自动延长锁的时间为timeoutSeconds。除非程序意外退出，否则永不超时。</param>
        /// <returns></returns>
        CSRedisClientLock Lock(string name, int timeoutSeconds, bool autoDelay = true);
    }
}
