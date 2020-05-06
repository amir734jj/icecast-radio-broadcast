using System;
using System.Threading.Tasks;
using Flurl;
using Logic.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Models.Options;
using Models.ViewModels.Stream;
using StructureMap;
using Stream = Models.ViewModels.Stream.Stream;

namespace Logic
{
    [Singleton]
    public class StreamSubscriptionClient : IStreamSubscriptionClient
    {
        private readonly StreamSubscriptionClientOptions _options;

        private HubConnection _connection;

        private readonly ILogger<StreamSubscriptionClient> _logger;
        
        private readonly IStreamSubscriptionAuth _subscriptionAuth;

        private readonly BroadcastServer _broadcastServer;

        private int Retries { get; set; } = 5;

        public StreamSubscriptionClient(StreamSubscriptionClientOptions options,
            IStreamSubscriptionAuth subscriptionAuth,
            BroadcastServer broadcastServer,
            ILogger<StreamSubscriptionClient> logger)
        {
            _options = options;
            _subscriptionAuth = subscriptionAuth;
            _broadcastServer = broadcastServer;
            _logger = logger;
        }

        public async Task Listen()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Url.Combine(_options.Url, _options.Path), x =>
                {
                    x.AccessTokenProvider = () => Task.FromResult(_subscriptionAuth.Token);
                })
                .Build();

            _connection.On("download", (string filename, SongMetadata metadata, string base64, Stream stream) =>
            {
                Console.WriteLine(filename);

                _broadcastServer.IceCastSend(base64);
            });

            _connection.Closed += async error =>
            {
                _logger.LogError("SignalR connection failed", error);

                Retries--;

                if (Retries > 0)
                {
                    await _connection.StartAsync();

                    Retries = 5;
                }
            };

            await _connection.StartAsync();
            
            _logger.LogInformation("Successfully started SignalR client");
        }
    }
}