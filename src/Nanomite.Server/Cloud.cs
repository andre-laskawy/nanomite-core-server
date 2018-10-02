///-----------------------------------------------------------------
///   File:         Cloud.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 17:00:15
///-----------------------------------------------------------------

namespace Nanomite.Server
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Nanomite.Server.Base;
    using Nanomite.Server.Base.Locator;
    using Nanomite.Server.Base.Handler;
    using Nanomite.Server.Helper;
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="Cloud" />
    /// </summary>
    public class Cloud
    {
        /// <summary>
        /// Gets or sets the running cloud.
        /// </summary>
        public static ICloud RunningCloud { get; set; }

        /// <summary>
        /// Gets or sets the source device.
        /// </summary>
        public static string SrcDeviceId { get; set; }

        /// <summary>
        /// Gets or sets the kestrel endpoint.
        /// </summary>
        public static IPEndPoint KestrelEndpoint { get; set; }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            Run();
        }

        /// <summary>
        /// Runs the grpc and kestrel host.
        /// </summary>
        /// <param name="startKestrelHost">The startKestrelHost<see cref="bool"/></param>
        /// <param name="startGrpcHost">The startGrpcHost<see cref="bool"/></param>
        /// <param name="customAssemblyPath">The customAssemblyPath<see cref="string"/></param>
        public static void Run(bool startKestrelHost = true,
            bool startGrpcHost = true,
            string customAssemblyPath = null)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

                //Locate cloud assembly and call register method on clound instance
                CloudLocator.Locate(customAssemblyPath);

                // Get cloud instance -> Decission is made dependinig on the assembly inside the bin folder
                RunningCloud = CloudLocator.GetCloud();

                // Get config
                IConfig config = CloudLocator.GetConfig();
                if (config == null)
                {
                    throw new Exception("No config registered");
                }

                // grpc host
                if (startGrpcHost)
                {
                    StartGrpcHost();
                }

                // kestrel host
                if (startKestrelHost)
                {
                    // Get the cloud address
                    KestrelEndpoint = RunningCloud.GetCloudAddress(config);

                    // start kestrel host
                    var host = new WebHostBuilder()
                        .UseKestrel()
                        .UseContentRoot(Directory.GetCurrentDirectory())
                        .UseStartup<Startup>()
                        .UseApplicationInsights()
                        .UseUrls(string.Format("http://{0}:{1}", KestrelEndpoint.Address.MapToIPv4(), config.PortApi))
                        .Build();

                    host.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        /// <summary>
        /// Starts the GRPC host.
        /// </summary>
        public static void StartGrpcHost()
        {
            IConfig config = CloudLocator.GetConfig();

            // start grpc host
            try
            {
                // Logging
                Logger.Initialize(config.SrcDeviceId);
                CommonBaseHandler.OnLog = (sender, msg, level) => { Logger.Log(sender, msg, level); };

                // Run Cloud
                new Task(async () =>
                {
                    await RunningCloud.Start(config);
                }).Start();
            }
            catch (Exception ex)
            {
                Logger.Log("CLOUD", ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// The CurrentDomain_UnhandledException
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="UnhandledExceptionEventArgs"/></param>
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Console.ReadLine();
        }
    }
}
