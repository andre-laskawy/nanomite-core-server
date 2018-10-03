///-----------------------------------------------------------------
///   File:         Program.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 16:59:56
///-----------------------------------------------------------------

namespace Nanomite.Core.Server
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    /// <summary>
    /// Defines the <see cref="Program" />
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        /// <summary>
        /// The BuildWebHost
        /// </summary>
        /// <param name="args">The args<see cref="string[]"/></param>
        /// <returns>The <see cref="IWebHost"/></returns>
        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
