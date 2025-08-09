// IMessageBus.cs
namespace OneCOverlayClient.Services.Interfaces;

public interface IMessageBus
{
    /// <summary>
    /// Подписывает асинхронный обработчик на сообщение определенного типа.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="handler">Асинхронный обработчик сообщения.</param>
    void Subscribe<TMessage>(Func<TMessage, Task> handler);

    /// <summary>
    /// Отписывает асинхронный обработчик от сообщения определенного типа.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="handler">Обработчик для удаления.</param>
    void Unsubscribe<TMessage>(Func<TMessage, Task> handler);

    /// <summary>
    /// Асинхронно публикует сообщение всем подписчикам.
    /// </summary>
    /// <typeparam name="TMessage">Тип сообщения.</typeparam>
    /// <param name="message">Сообщение для отправки.</param>
    /// <returns>Задача, завершающаяся после обработки всеми подписчиками.</returns>
    Task Publish<TMessage>(TMessage message);
}