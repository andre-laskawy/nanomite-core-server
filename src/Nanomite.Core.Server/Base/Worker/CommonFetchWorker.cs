///-----------------------------------------------------------------
///   File:         CommonFetchWorker.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:21:13
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Worker
{
    using Grpc.Core;
    using Nanomite.Core.Network.Grpc;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CommonFetchWorker"/>
    /// </summary>
    public abstract class CommonFetchWorker : CommonBaseWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommonFetchWorker"/> class.
        /// </summary>
        public CommonFetchWorker() : base()
        { }

        /// <inheritdoc />
        public abstract Task<GrpcResponse> ProcessFetch(FetchRequest message, string streamId, string token, Metadata header, bool checkForAuthentication = true);
    }
}
