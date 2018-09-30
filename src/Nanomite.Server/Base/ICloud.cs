namespace Nanomite.Server.Base
{
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="ICloud"/>
    /// </summary>
    public interface ICloud
    {
        /// <summary>
        /// Registers the cloud to the global cloud project. This method is called by reflection
        /// call, because the assemby is loaded via late binding.
        /// </summary>
        void Register();

        /// <summary>
        /// Starts the specified cloud endpoint.
        /// </summary>
        /// <param name="connectToOnPremiseCloud">
        /// if set to <c>true</c> [connect to on premise cloud].
        /// </param>
        /// <returns>The <see cref="Task"/></returns>
        Task Start(IConfig config, bool connectToOnPremiseCloud = true);

        /// <summary>
        /// Gets the cloud address.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>the ip endpoint</returns>
        IPEndPoint GetCloudAddress(IConfig config);

        /// <summary>
        /// Add custom middlewares to the asp net core host.
        /// </summary>
        /// <param name="app">the application builder</param>
        /// <param name="env">the host enviroment</param>
        void AddMiddlewares(dynamic app, dynamic env);

        /// <summary>
        /// Add custom services to the asp net core host.
        /// </summary>
        /// <param name="serviceCollection">the service collection</param>
        void AddServices(dynamic serviceCollection);
    }
}