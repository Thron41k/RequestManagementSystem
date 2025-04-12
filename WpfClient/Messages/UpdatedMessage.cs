using RequestManagement.Common.Models.Interfaces;

namespace WpfClient.Messages
{
    public class UpdatedMessage(MessagesEnum message,Type? caller = null)
    {
        public MessagesEnum Message { get; } = message;
    }
    public class SelectTaskMessage(MessagesEnum message, Type caller)
    {
        public MessagesEnum Message { get; } = message;
        public Type Caller { get; } = caller;
    }
    public class SelectResultMessage(MessagesEnum message, Type caller, IEntity? item = null)
    {
        public MessagesEnum Message { get; } = message;
        public Type Caller { get; } = caller;
        public IEntity? Item { get; } = item;
    }
}
