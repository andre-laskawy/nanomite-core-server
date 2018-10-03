///-----------------------------------------------------------------
///   File:         BaseBroker.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:21:20
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Broker
{
    using Nanomite.Core.Server.Base.Worker;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="BaseBroker"/>
    /// </summary>
    public abstract class BaseBroker : ICloud
    {
        /// <summary>
        /// Gets or sets the action worker.
        /// </summary>
        public CommonActionWorker ActionWorker { get; set; }

        /// <summary>
        /// Gets or sets the FetchWorker
        /// The fetch worker
        /// </summary>
        public CommonFetchWorker FetchWorker { get; set; }

        /// <summary>
        /// Registers the cloud to the global cloud project. This method is called by reflection
        /// call, because the assemby is loaded via late binding.
        /// </summary>
        public abstract void Register();

        /// <summary>
        /// Starts the specified cloud endpoint.
        /// </summary>
        /// <param name="config"></param>
        /// <returns>The <see cref="Task"/></returns>
        public abstract Task Start(IConfig config);

        /// <summary>
        /// Add custom middlewares to the asp net core host.
        /// </summary>
        /// <param name="app">the application builder</param>
        /// <param name="env">the host enviroment</param>
        public virtual void AddMiddlewares(dynamic app, dynamic env)
        {
        }

        /// <summary>
        /// Add custom services to the asp net core host.
        /// </summary>
        /// <param name="serviceCollection">the service collection</param>
        public virtual void AddServices(dynamic serviceCollection)
        {
        }

        /// <summary>
        /// Gets the cloud address.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public IPEndPoint GetCloudAddress(IConfig config)
        {
            // Get server address
            string ipAddress = config.LocalCloudAddress;
            IPAddress host = null;
            if (string.IsNullOrEmpty(ipAddress))
            {
                IPHostEntry entry = Dns.GetHostEntry(Dns.GetHostName());
                host = entry.AddressList.LastOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork);
            }
            else
            {
                host = IPAddress.Parse(ipAddress);
            }

            // local endpoints
            return new IPEndPoint(host, config.PortGrpc);
        }
    }
}
