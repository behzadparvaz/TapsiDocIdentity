using IdentityTapsiDoc.Identity.Core.Domain.Users.Entities;
using IdentityTapsiDoc.Identity.Core.Domain.Users.Repositories;
using IdentityTapsiDoc.Identity.Infra.Data.Command.Users.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;

namespace IdentityTapsiDoc.Identity.Infra.Data.Command.Users
{
    public class UserCommandRepository : IUserCommandRepository
    {
        private string _baseSendSms = "BaseUrlSendSms";
        private static TimeoutPolicy _timeoutPolicy;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
        private readonly AsyncFallbackPolicy<HttpResponseMessage> _fallbackPolicy;
        private static Polly.CircuitBreaker.AsyncCircuitBreakerPolicy<HttpResponseMessage> _circuitBreaker =
            Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode).Or<HttpRequestException>()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(60),
                (d, c) =>
                {
                    string a = "Break";
                },
                () =>
                {
                    string a = "Reset";
                },
                () =>
                {
                    string a = "Half";
                });

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };
        private readonly IConfiguration configuration;
        private readonly IUserCommandRepositoryRedis redisManager;

        public UserCommandRepository(IConfiguration configuration, IUserCommandRepositoryRedis redisManager)
        {
            _timeoutPolicy = Policy.Timeout(200, TimeoutStrategy.Pessimistic);

            _retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .RetryAsync(1, (d, c) =>
                {
                    string a = "Retry";
                });

            _fallbackPolicy = Policy.HandleResult<HttpResponseMessage>(result => (int)result.StatusCode == 500)
                .Or<BrokenCircuitException>()
                .FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new ObjectContent(typeof(Message), new Message
                    {
                        Id = 100,
                        Text = "Retry Send Data"
                    }, new JsonMediaTypeFormatter())
                });
            this.configuration = configuration;
            this.redisManager = redisManager;
        }
        public Task Create(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SendOtpCode(string phoneNumber)
        {
            try
            {
                var client = new HttpClient();
                Random generator = new Random();
                string rand = generator.Next(0, 1000000).ToString("D6");
                if (rand.Length <= 5 )
                    rand = rand.PadRight(6, '1');

                Model model = new()
                {
                    PhoneNumber = phoneNumber,
                    OtpCode = rand
                };
                var json = JsonConvert.SerializeObject(model, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() });
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var result = await _fallbackPolicy.ExecuteAsync(() => _retryPolicy.ExecuteAsync(() =>
                                                                      _circuitBreaker.ExecuteAsync(() =>
                                                                       client.PostAsync($"{this.configuration.GetSection("SMS").GetSection(_baseSendSms).Value}", content)
                                                                          )));
                if (result.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    this.redisManager.Create(phoneNumber, rand.ToString(), TimeSpan.FromMinutes(3));
                    return true;
                }
                else
                    throw new ArgumentException("Send SMS", "SMS Code Error, Please Try Agin");
            }
            catch (Exception ex)
            {

                throw new ArgumentException("خطایی رخ داده است لطفا چند لحظه بعد مجدد تلاش نمایید" , ex.Message);
            }

        }
    }
}
