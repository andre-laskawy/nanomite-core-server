///-----------------------------------------------------------------
///   File:         Startup.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 16:59:48
///-----------------------------------------------------------------

namespace Nanomite.Core.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Cors.Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Nanomite.Core.Server.Base.Locator;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Service Startup class.
    /// Used for initialization of the service.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="env">The env.</param>
        /// <param name="configuration">The configuration.</param>
        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            // Set up configuration
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        /// <summary>
        /// Gets the Configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">Collection of services</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Add CORS
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin();
            corsBuilder.AllowCredentials();
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", corsBuilder.Build());
            });

            // Expose the Configuration
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add custom services
            Cloud.RunningCloud.AddServices(services);
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The ApplicationBuilder</param>
        /// <param name="env">The Environment</param>
        /// <param name="loggerFactory">The LoggerFactory</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddDebug();
            app.UseStaticFiles();

            // Activate CORS
            app.UseCors("AllowAll");

            // add custom middlewares
            Cloud.RunningCloud.AddMiddlewares(app, env);
        }
    }
}
