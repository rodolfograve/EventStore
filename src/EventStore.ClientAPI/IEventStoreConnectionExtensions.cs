using System;
using System.Threading.Tasks;
using EventStore.ClientAPI.SystemData;

namespace EventStore.ClientAPI
{
    /// <summary>
    /// Extensions for <seealso cref="IEventStoreConnection"/>
    /// </summary>
    public static class IEventStoreConnectionExtensions
    {
        /// <summary>
        /// Asynchronously subscribes to a single event stream. New events
        /// written to the stream while the subscription is active will be
        /// pushed to the client.
        /// </summary>
        /// <param name="target">The connection to subscribe to</param>
        /// <param name="stream">The stream to subscribe to</param>
        /// <param name="resolveLinkTos">Whether to resolve Link events automatically</param>
        /// <param name="eventAppeared">A Task invoked and awaited when a new event is received over the subscription</param>
        /// <param name="subscriptionDropped">An action invoked if the subscription is dropped</param>
        /// <param name="userCredentials">User credentials to use for the operation</param>
        /// <returns>An <see cref="EventStoreSubscription"/> representing the subscription</returns>
        public static Task<EventStoreSubscription> SubscribeToStreamAsync(
                this IEventStoreConnection target,
                string stream,
                bool resolveLinkTos,
                Action<EventStoreSubscription, ResolvedEvent> eventAppeared,
                Action<EventStoreSubscription, SubscriptionDropReason, Exception> subscriptionDropped = null,
                UserCredentials userCredentials = null)
        {
            return target.SubscribeToStreamAsync(
                stream,
                resolveLinkTos,
                (subscription, e) =>
                {
                    eventAppeared(subscription, e);
                    return Task.CompletedTask;
                },
                subscriptionDropped,
                userCredentials
                );
        }
    }
}
