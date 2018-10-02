///-----------------------------------------------------------------
///   File:         CommonActionWorker.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:21:12
///-----------------------------------------------------------------

namespace Nanomite.Server.Base.Worker
{
    using Grpc.Core;
    using Nanomite.Services.Network.Common;
    using Nanomite.Services.Network.Grpc;
    using System;
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
