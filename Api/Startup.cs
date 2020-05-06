using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Api.Config;
using Logic;
using Logic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Models.Options;
using RestSharp;
using StructureMap;

namespace Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.AddResponseCompression();

            services.Configure<StreamSubscriptionOptions>(_configuration.GetSection("StreamSubscriptionOptions"));
            
            var streamSubscriptionOptions = _configuration
                .GetSection("StreamSubscriptionOptions")
                .Get<StreamSubscriptionOptions>();

            var container = new Container(config =>
            {
                config.For<RestClient>().Use(new RestClient(streamSubscriptionOptions.Host));
                
                config.For<StreamSubscriptionAuthOptions>().Use(new StreamSubscriptionAuthOptions(
                    streamSubscriptionOptions.LoginRoute,
                    streamSubscriptionOptions.Username,
                    streamSubscriptionOptions.Password));

                config.For<StreamSubscriptionClientOptions>()
                    .Use(new StreamSubscriptionClientOptions(streamSubscriptionOptions.Host,
                        streamSubscriptionOptions.StreamRoute));

                config.For<BroadcastOptions>().Use(new BroadcastOptions
                {
                    Address = IPAddress.Any,
                    Port = 8080
                });
                
                
                // Register stuff in container, using the StructureMap APIs...
                config.Scan(_ =>
                {
                    _.AssemblyContainingType(typeof(Startup));
                    _.Assembly("Api");
                    _.Assembly("Logic");
                    _.WithDefaultConventions();
                });

                // Populate the container using the service collection
                config.Populate(services);
            });

            container.AssertConfigurationIsValid();

            return container.GetInstance<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IStreamSubscriptionAuth subscriptionAuth,
            IBroadcastServer broadcastServer,
            IStreamSubscriptionClient subscriptionClient)
        {
            subscriptionAuth.ResolveToken();
            
            broadcastServer.Start();
            
            subscriptionClient.Listen().Wait();
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}