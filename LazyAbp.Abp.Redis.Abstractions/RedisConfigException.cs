using System;
using System.Collections.Generic;
using System.Text;

namespace LazyAbp.Abp.Redis.Abstractions
{
    public class RedisConfigException : Exception
    {
        public RedisConfigException() : base()
        {

        }

        public RedisConfigException(string message) : base(message)
        {

        }

        public RedisConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
