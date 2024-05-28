using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Infra.Data.Command.Users
{
    public class UserCommandRepositoryRedis : IUserCommandRepositoryRedis
    {
        private readonly IRedisClient redisClient;
        private HashSet<string> _keys;
        public UserCommandRepositoryRedis(IRedisClientsManager clientsManager)
        {
            using (IRedisClient redisClient = clientsManager.GetClient())
                this.redisClient = redisClient;
            var res = redisClient.Custom(ServiceStack.Redis.Commands.Keys, "*");
            _keys = res.Children.Select(x => x.Text).ToHashSet<string>();
            if (_keys == null)
                _keys = new HashSet<string>();
        }
        public bool Create<T>(string cacheKey, T value, TimeSpan time)
        {
            redisClient.Remove(cacheKey);
            var result = redisClient.Set(cacheKey, value, time);
            if (result)
                _keys.Add(cacheKey);
            return result;
        }
    }
}
