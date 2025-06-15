using Wim.Abstractions;

namespace Wim
{
    internal class MessageManager : IMessageManager
    {
        private readonly Dictionary<string, List<Action<object>>> _subscribers = [];

        /// <summary>
        /// Subscribes to a specific message type and registers a callback to be invoked when a message of that type is
        /// received.
        /// </summary>
        /// <remarks>If a subscription for the specified <paramref name="messageType"/> already exists,
        /// the existing callbacks will be replaced.</remarks>
        /// <param name="messageType">The type of message to subscribe to. This must be a non-null, non-empty string that uniquely identifies the
        /// message type.</param>
        /// <param name="callback">The action to execute when a message of the specified type is received. This must be a non-null delegate.</param>
        public void Subscribe(string messageType, Action<object> callback)
        {
            if (_subscribers.ContainsKey(messageType))
            {
                _subscribers[messageType] = [];
            }
            _subscribers[messageType].Add(callback);
        }

        /// <summary>
        /// Removes a previously registered callback for the specified message type.
        /// </summary>
        /// <remarks>If the specified callback is not found for the given message type, no action is
        /// taken. If the last callback for a message type is removed, the message type is also removed from the
        /// subscription list.</remarks>
        /// <param name="messageType">The type of message for which the callback was registered. Cannot be null or empty.</param>
        /// <param name="callback">The callback to be removed. Cannot be null.</param>
        public void Unsubscribe(string messageType, Action<object> callback)
        {
            if (_subscribers.TryGetValue(messageType, out var callbacks))
            {
                callbacks.Remove(callback);
                if (callbacks.Count == 0)
                {
                    _subscribers.Remove(messageType);
                }
            }
        }

        /// <summary>
        /// Removes all subscribers from the current list of subscriptions.
        /// </summary>
        /// <remarks>This method clears the internal collection of subscribers, effectively removing all
        /// registered subscriptions. After calling this method, no subscribers will be notified of events until new
        /// subscriptions are added.</remarks>
        public void ClearSubscriptions()
        {
            _subscribers.Clear();
        }

        /// <summary>
        /// Publishes a message to all subscribers of the specified message type.
        /// </summary>
        /// <remarks>If there are no subscribers for the specified <paramref name="messageType"/>, the
        /// method performs no action. Subscribers are invoked in the order they were added.</remarks>
        /// <param name="messageType">The type of the message to publish. This is used to identify the subscribers that should receive the
        /// message.</param>
        /// <param name="message">The message to be delivered to the subscribers. This can be any object representing the data associated with
        /// the message.</param>
        public void NotifyAll(string messageType, object message)
        {
            if (_subscribers.TryGetValue(messageType, out var callbacks))
            {
                foreach (var callback in callbacks)
                {
                    callback(message);
                }
            }
        }


        /// <summary>
        /// Notifies the first subscriber of the specified message type with the provided message.
        /// </summary>
        /// <remarks>This method retrieves the first subscriber associated with the specified <paramref
        /// name="messageType"/>  and invokes its callback with the provided <paramref name="message"/>. If no
        /// subscribers exist for the  given message type, the method does nothing.</remarks>
        /// <param name="messageType">The type of the message to notify subscribers about. Cannot be null or empty.</param>
        /// <param name="message">The message object to pass to the subscriber. Can be any object relevant to the message type.</param>
        public void NotifyOne(string messageType, object message)
        {
            if (_subscribers.TryGetValue(messageType, out var callbacks) && callbacks.Count > 0)
            {
                callbacks[0](message);
            }
        }
    }
}
