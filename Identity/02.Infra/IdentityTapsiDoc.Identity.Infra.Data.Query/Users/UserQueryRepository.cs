using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Infra.Data.Query.Users
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly IUserQueryRepositoryRedis repositoryRedis;

        public UserQueryRepository(IUserQueryRepositoryRedis repositoryRedis)
        {
            this.repositoryRedis = repositoryRedis;
        }
        public Task<bool> CheckCode(string phoneNumber, string code)
        {
            var result = this.repositoryRedis.Get<string>(phoneNumber);
            if (result.Equals(code))
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }

        public Task<bool> CheckSendSMS(string phoneNumber)
        {
            var result = this.repositoryRedis.Get<string>(phoneNumber);
            if (!string.IsNullOrEmpty(result))
                return Task.FromResult(true);
            else
                return Task.FromResult(false);
        }
    }
}
