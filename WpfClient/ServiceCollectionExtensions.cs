using Microsoft.Extensions.DependencyInjection;
using RequestManagement.WpfClient.Models;
using RequestManagement.WpfClient.Services;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.Services.Message;

namespace RequestManagement.WpfClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, MessageBusService>();
        services.AddScoped<PrintReportMaterialsInUseOffTaskHandlerService>();
        services.AddSingleton<MessageProcessingService>();

        return services;
    }

    public static void ConfigureMessageHandlers(this IServiceProvider serviceProvider)
    {
        var processor = serviceProvider.GetRequiredService<MessageProcessingService>();
        processor.RegisterHandler<PrintMaterialInUseOffModel, PrintReportMaterialsInUseOffTaskHandlerService>();
    }
}