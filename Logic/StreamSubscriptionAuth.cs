using System;
using System.Threading;
using Logic.Interfaces;
using Models.Options;
using Models.ViewModels;
using RestSharp;
using StructureMap;

namespace Logic
{
    [Singleton]
    public class StreamSubscriptionAuth : IDisposable, IStreamSubscriptionAuth
    {
        private readonly StreamSubscriptionAuthOptions _options;

        private readonly RestClient _client;

        private Timer _timer;
        
        public string Token { get; private set; }

        public StreamSubscriptionAuth(RestClient client, StreamSubscriptionAuthOptions options)
        {
            _client = client;
            _options = options;
        }

        public void ResolveToken()
        {
            var request = new RestRequest(_options.Url)
                .AddJsonBody(new LoginRequestViewModel
                {
                    Username = _options.Username,
                    Password = _options.Password
                });

            var response = _client.Post<LoginResponseViewModel>(request);

            Token = response.Data.Token;

            _timer = new Timer(x => ResolveToken(),
                null,
                response.Data.Expires.Subtract(DateTimeOffset.Now).Subtract(TimeSpan.FromMinutes(5)),
                Timeout.InfiniteTimeSpan
            );
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}