using RequestManagement.WpfClient.Services.Interfaces;
using System.Text.Json;

namespace RequestManagement.WpfClient.Services;

public class MessageBusService : IMessageBus
{
    private readonly Dictionary<Type, List<object>> _subscribers = new();
    public void Subscribe<TMessage>(Func<TMessage, Task> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        if (!_subscribers.ContainsKey(messageType))
        {
            _subscribers[messageType] = [];
        }

        _subscribers[messageType].Add(handler);
    }
    public void Unsubscribe<TMessage>(Func<TMessage, Task> handler)
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        if (!_subscribers.TryGetValue(messageType, out var subscriber)) return;
        subscriber.Remove(handler);
        if (_subscribers[messageType].Count == 0)
        {
            _subscribers.Remove(messageType);
        }
    }
    public async Task Publish<TMessage>(TMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        if (_subscribers.TryGetValue(messageType, out var subscriber))
        {
            var tasks = (from Func<TMessage, Task> asyncHandler in subscriber.ToArray() select asyncHandler(message)).ToList();
            await Task.WhenAll(tasks);
        }
    }

    public async Task<IReadOnlyCollection<TMessage>> Request<TMessage>(TMessage message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        if (!_subscribers.TryGetValue(messageType, out var subscriber) || subscriber.Count == 0)
            return [];

        var tasks = subscriber
            .Cast<Func<TMessage, Task>>()
            .Select(async handler =>
            {
                var clone = CloneMessage(message);
                await handler(clone);
                return clone;
            });

        return await Task.WhenAll(tasks);
    }

    private static T CloneMessage<T>(T source)
    {
        var json = JsonSerializer.Serialize(source);
        return JsonSerializer.Deserialize<T>(json)!;
    }
}