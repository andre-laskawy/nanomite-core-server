namespace Nanomite.Server.Base
{
    /// <summary>
    /// Defines the <see cref="IConfig"/>
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets the local cloud address.
        /// </summary>
        string LocalCloudAddress { get; set; }

        /// <summary>
        /// Gets the port API.
        /// </summary>
        int PortApi { get; set; }

        /// <summary>
        /// Gets the port GRPC.
        /// </summary>
        int PortGrpc { get; set; }

        /// <summary>
        /// Gets the source device identifier.
        /// </summary>
        string SrcDeviceId { get; set; }
    }
}