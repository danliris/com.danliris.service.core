using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Lib.Helpers
{
    public class RedisCache : IRedisCache
    {
        Lazy<ConnectionMultiplexer> _connection;

        public RedisCache(string connectionString)
        {
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));

        }

        public async Task<string> StringGetAsync(string key)
        {
            var database = _connection.Value.GetDatabase();
            
            string keyString = key?.ToString() ?? "null";
            var result = await database.StringGetAsync(keyString);
            return result.ToString();
        }

        public async Task<string> StringSetAsync(string key, string value)
        {
            var database = _connection.Value.GetDatabase();
            var result = await database.StringSetAsync(key, value);
            return result.ToString();
        }
    }
}
