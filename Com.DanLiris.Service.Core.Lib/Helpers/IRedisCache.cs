using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Com.DanLiris.Service.Core.Lib.Helpers
{
    public interface IRedisCache
    {
        Task<string> StringGetAsync(string key);

        Task<string> StringSetAsync(string key, string value);
    }
}
