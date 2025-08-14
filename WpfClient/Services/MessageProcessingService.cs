using Microsoft.Extensions.DependencyInjection;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.Services;

public class MessageProcessingService(IMessageBus messageBus, IServiceProvider serviceProvider)
{
    private readonly IMessageBus _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    public void RegisterHandler<TMessage, THandler>()
        where THandler : class, IMessageHandlerService<TMessage>
    {
        try
        {
            _messageBus.Subscribe<TMessage>(async message =>
            {
                using var scope = _serviceProvider.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<THandler>();
                await handler.HandleAsync(message);
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка регистрации обработчика {typeof(THandler).Name}: {ex}");
        }
    }
}