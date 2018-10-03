///-----------------------------------------------------------------
///   File:         IConfig.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:44:02
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base
{
    /// <summary>
    /// Defines the <see cref="IConfig"/>
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets the LocalCloudAddress
        /// Gets the local cloud address.
        /// </summary>
        string LocalCloudAddress { get; set; }

        /// <summary>
        /// Gets or sets the PortApi
        /// Gets the port API.
        /// </summary>
        int PortApi { get; set; }

        /// <summary>
        /// Gets or sets the PortGrpc
        /// Gets the port GRPC.
        /// </summary>
        int PortGrpc { get; set; }

        /// <summary>
        /// Gets or sets the SrcDeviceId
        /// Gets the source device identifier.
        /// </summary>
        string SrcDeviceId { get; set; }
    }
}
