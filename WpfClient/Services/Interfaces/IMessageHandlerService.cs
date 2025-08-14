namespace RequestManagement.WpfClient.Services.Interfaces;
public interface IMessageHandlerService<in TMessage>
{
    Task HandleAsync(TMessage message);
}
