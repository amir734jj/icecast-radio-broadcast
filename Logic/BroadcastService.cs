using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Logic.Interfaces;
using Microsoft.Extensions.Logging;
using Models.Options;
using NetCoreServer;
using StructureMap;

namespace Logic
{
    internal class ChatSession : TcpSession
    {
        private readonly ILogger _logger;

        public ChatSession(TcpServer server, ILogger logger) : base(server)
        {
            _logger = logger;
        }

        protected override void OnConnected()
        {
            _logger.LogTrace($"Chat TCP session with Id {Id} connected!");
        }

        protected override void OnDisconnected()
        {
            _logger.LogTrace($"Chat TCP session with Id {Id} disconnected!");
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Chat TCP session caught an error with code {error}");
        }
    }

    [Singleton]
    public class BroadcastServer : TcpServer, IBroadcastServer
    {
        private readonly ILogger<BroadcastServer> _logger;
        
        private readonly IHttpResponseBuilder _httpResponseBuilder;

        public BroadcastServer(BroadcastOptions options, 
            IHttpResponseBuilder httpResponseBuilder,
            ILogger<BroadcastServer> logger) : base(options.Address, options.Port)
        {
            _httpResponseBuilder = httpResponseBuilder;
            _logger = logger;
        }

        protected override TcpSession CreateSession()
        {
            return new ChatSession(this, _logger);
        }

        protected override void OnError(SocketError error)
        {
            _logger.LogError($"Chat TCP server caught an error with code {error}");
        }

        public void IceCastSend(string data)
        {
            var message = _httpResponseBuilder.Build(200, "OK", new Dictionary<string, string>
            {
                {"content-type", "audio/mpeg"},
                {"ice-name", "lala"},
                {"ice-url", "localhost"}
            }, data);
            
            Multicast(message);
        }
    }

    public class IceCastStream
    {
        public void Amir()
        {
            var sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            sck.Connect("localhost", 8080);
            sck.Send(Encoding.UTF8.GetBytes("THE HTTP REQUEST HEADER"));

            while (true)
            {
                sck.Send(Encoding.ASCII.GetBytes("Hello world\n"));
                Console.WriteLine("SENT");
            }

            /*
            var socketServer = new TcpClient("localhost", 8080);
            var networkStream = socketServer.GetStream();

            var data = Encoding.ASCII.GetBytes("SOURCE /csharp ICE/1.0");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("content-type: audio/mpeg");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("Authorization: Basic c291cmNlOmhhY2ttZQ==");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-name: lala");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-url: localhost");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-genre: Rock");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-bitrate: 128");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-private: 0");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-public: 1");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-description: This is my server description");
            networkStream.Write(data, 0, data.Length);

            data = Encoding.ASCII.GetBytes("ice-audio-info: ice-samplerate=44100;ice-bitrate=128;ice-channels=2");
            networkStream.Write(data, 0, data.Length);

            var reader = new StreamReader(networkStream);
            Console.WriteLine(reader.ReadToEnd());*/
        }
    }
}