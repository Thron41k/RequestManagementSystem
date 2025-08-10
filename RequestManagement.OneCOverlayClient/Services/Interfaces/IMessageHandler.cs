namespace OneCOverlayClient.Services.Interfaces
{
    public interface IMessageHandler<in TMessage>
    {
        Task HandleAsync(TMessage message);
    }
}
