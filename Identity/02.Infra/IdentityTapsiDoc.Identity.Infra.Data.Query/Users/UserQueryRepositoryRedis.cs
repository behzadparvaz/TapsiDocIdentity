using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using ServiceStack.Redis;

namespace IdentityTapsiDoc.Identity.Infra.Data.Query.Users
{
    public class UserQueryRepositoryRedis : IUserQueryRepositoryRedis
    {
        private readonly IRedisClient redisClient;
        private HashSet<string> _keys;
        public UserQueryRepositoryRedis(IRedisClientsManager clientsManager)
        {
            using (IRedisClient redisClient = clientsManager.GetClient())
                this.redisClient = redisClient;
            var res = redisClient.Custom(ServiceStack.Redis.Commands.Keys, "*");
            _keys = res.Children.Select(x => x.Text).ToHashSet<string>();
            if (_keys == null)
                _keys = new HashSet<string>();
        }
        public T Get<T>(string key)
        {
            try
            {
                var result = redisClient.Get<string>(key);
                if (result == null)
                    return default(T);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(result);
            }
            catch
            {
                return default(T);
            }
        }

        public Task<List<string>> GetAll()
        {
            throw new NotImplementedException();
        }

        public string GetString(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetToken(string key)
        {
            throw new NotImplementedException();
        }
    }
}
