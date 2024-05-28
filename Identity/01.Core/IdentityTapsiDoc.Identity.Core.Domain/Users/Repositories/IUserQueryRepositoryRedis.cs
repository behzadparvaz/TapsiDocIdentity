using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories
{
    public interface IUserQueryRepositoryRedis
    {
        T Get<T>(string key);
        Task<string> GetToken(string key);
        Task<List<string>> GetAll();
        string GetString(string key);
    }
}
