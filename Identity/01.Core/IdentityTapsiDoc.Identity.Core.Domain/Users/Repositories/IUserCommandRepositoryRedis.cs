using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories
{
    public interface IUserCommandRepositoryRedis
    {
        bool Create<T>(string cacheKey, T value, TimeSpan time);
    }
}
