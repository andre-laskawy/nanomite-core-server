///-----------------------------------------------------------------
///   File:         StreamSubscription.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:46:45
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Helper
{
    using Nanomite.Core.Network.Common;
    using Nanomite.Core.Network.Grpc;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="StreamSubscription"/>
    /// </summary>
    public class StreamSubscription
    {
        /// <summary>
        /// The topics
        /// </summary>
        private List<string> topis = new List<string>();

        /// <summary>
        /// Gets or sets the topics.
        /// </summary>
        public List<string> Topics
        {
            get { return topis; }
            set { topis = value; }
        }

        /// <summary>
        /// Gets or sets the Stream
        /// </summary>
        public IStream<Command> Stream { get; set; }

        /// <summary>
        /// Gets or sets the UserId
        /// </summary>
        public string UserId { get; set; }
    }
}
