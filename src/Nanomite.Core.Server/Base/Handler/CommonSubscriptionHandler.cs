///-----------------------------------------------------------------
///   File:         CommonSubscriptionHandler.cs
///   Author:   	Andre Laskawy           
///   Date:         30.09.2018 18:47:43
///-----------------------------------------------------------------

namespace Nanomite.Core.Server.Base.Handler
{
    using Nanomite.Core.Server.Helper;
    using Nanomite.Core.Network;
    using Nanomite.Core.Network.Common;
    using Nanomite.Core.Network.Grpc;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the <see cref="CommonSubscriptionHandler"/>
    /// </summary>
    public static class CommonSubscriptionHandler
    {
        /// <summary>
        /// The subscriptions
        /// </summary>
        private static ConcurrentDictionary<string, StreamSubscription> subscriptions = new ConcurrentDictionary<string, StreamSubscription>();

        /// <summary>
        /// Adds the specified user to the cache.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="userId">The userId<see cref="string"/></param>
        public static void RegisterStream(IStream<Command> stream, string userId)
        {
            StreamSubscription subscriptionStream = null;
            if (!subscriptions.ContainsKey(stream.Id))
            {
                subscriptionStream = new StreamSubscription()
                {
                    Stream = stream,
                    UserId = userId
                };

                while (!subscriptions.TryAdd(stream.Id, subscriptionStream))
                {
                    Task.Delay(1).Wait();
                }
            }
            else
            {
                subscriptionStream = subscriptions[stream.Id];
                subscriptionStream.Stream = stream;
                subscriptionStream.UserId = userId;
            }
        }

        /// <summary>
        /// Removes the specified stream from the cache.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <returns>The <see cref="Task"/></returns>
        public static async Task UnregisterStream(string streamId)
        {
            if (subscriptions.ContainsKey(streamId))
            {
                StreamSubscription subscription = null;
                while (!subscriptions.TryRemove(streamId, out subscription))
                {
                    await Task.Delay(1);
                }
            }
        }

        /// <summary>
        /// Subscribe to a topic.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="topic">The topic.</param>
        public static void SubscribeToTopic(string streamId, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                StreamSubscription subscriptionStream = subscriptions[streamId];
                if (!subscriptionStream.Topics.Contains(topic))
                {
                    subscriptionStream.Topics.Add(topic);
                }
            }
        }

        /// <summary>
        /// Unsubscribes from a topic.
        /// </summary>
        /// <param name="streamId">The stream identifier.</param>
        /// <param name="topic">The topic.</param>
        public static void UnsubscribeFromTopic(string streamId, string topic)
        {
            if (!string.IsNullOrEmpty(topic))
            {
                StreamSubscription subscriptionStream = subscriptions[streamId];
                if (subscriptionStream.Topics.Contains(topic))
                {
                    subscriptionStream.Topics.Remove(topic);
                }
            }
        }

        /// <summary>
        /// Forwards by the specific topic. 
        /// If the command target is set, the topic will be combinded with target identifier.
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="topic">The topic.</param>
        public static void ForwardByTopic(Command cmd, string topic)
        {
            List<StreamSubscription> streams = subscriptions
                .Where(p => p.Value.Topics.Contains(topic))
                .Select(p => p.Value)
                .ToList();
            ForwardCommand(streams, cmd);
        }

        /// <summary>
        /// Forwards by the specific the topic and waits until another command 
        /// with the same key and target definded by the intial source is received. 
        /// </summary>
        /// <param name="srcId">The source identifier.</param>
        /// <param name="cmd">The command.</param>
        /// <param name="topic">The topic.</param>
        public static async Task<Command> ForwardByTopicAndWaitForResponse(string srcId, Command cmd, string topic, int timeout = 60)
        {
            cmd.SenderId = srcId;
            ForwardByTopic(cmd, topic);
            return await AsyncCommandProcessor.ProcessCommand(cmd, timeout);
        }

        /// <summary>
        /// Forwards the message.
        /// </summary>
        /// <param name="streams">The streams.</param>
        /// <param name="cmd">The cmd<see cref="Command"/></param>
        private static void ForwardCommand(List<StreamSubscription> streams, Command cmd)
        {
            foreach (var s in streams)
            {
                CommonBaseHandler.Log(typeof(CommonSubscriptionHandler).ToString(), "Forward package to stream of: " + s.UserId, NLog.LogLevel.Trace);
                s.Stream.AddToQueue(cmd);
            }
        }
    }
}
