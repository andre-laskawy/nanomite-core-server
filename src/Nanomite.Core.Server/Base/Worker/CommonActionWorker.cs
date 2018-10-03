///-----------------------------------------------------------------
///   File:         CommonActionWorker.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:21:12
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Worker
{
    using Grpc.Core;
    using Nanomite.Core.Network.Common;
    using Nanomite.Core.Network.Grpc;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CommonActionWorker"/>
    /// </summary>
    public abstract class CommonActionWorker : CommonBaseWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonActionWorker"/> class.
        /// </summary>
        public CommonActionWorker() : base()
        { }

        /// <inheritdoc />
        public abstract Task<GrpcResponse> StreamConnected(IStream<Command> stream, string token, Metadata header);

        /// <inheritdoc />
        public abstract Task<GrpcResponse> StreamDisconnected(string streamID);

        /// <inheritdoc />
        public abstract Task<GrpcResponse> ProcessCommand(string cloudId, Command cmd, string streamdId, string token, Metadata header, bool checkForAuthentication = true);
    }
}
