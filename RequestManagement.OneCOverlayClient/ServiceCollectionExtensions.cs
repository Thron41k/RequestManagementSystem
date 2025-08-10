using Microsoft.Extensions.DependencyInjection;
using OneCOverlayClient.Services;
using OneCOverlayClient.Services.Interfaces;
using OneCOverlayClient.Messages.Handlers;
using OneCOverlayClient.Messages.Models;

namespace OneCOverlayClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, MessageBusService>();
        services.AddScoped<SelectDriverTaskHandler>();
        services.AddSingleton<MessageProcessingService>();

        return services;
    }

    public static void ConfigureMessageHandlers(this IServiceProvider serviceProvider)
    {
        var processor = serviceProvider.GetRequiredService<MessageProcessingService>();
        processor.RegisterHandler<SelectDriverTaskModel, SelectDriverTaskHandler>();
    }
}