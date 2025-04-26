using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

public class GrpcClientFactory(IServiceProvider serviceProvider) : IGrpcClientFactory
{
    public AuthService.AuthServiceClient CreateAuthClient()
    {
        return serviceProvider.GetRequiredService<AuthService.AuthServiceClient>();
    }

    public RequestService.RequestServiceClient CreateRequestClient()
    {
        return serviceProvider.GetRequiredService<RequestService.RequestServiceClient>();
    }
    public StockService.StockServiceClient CreateStockClient()
    {
        return serviceProvider.GetRequiredService<StockService.StockServiceClient>();
    }

    public ExpenseService.ExpenseServiceClient CreateExpenseClient()
    {
        return serviceProvider.GetRequiredService<ExpenseService.ExpenseServiceClient>();
    }

    public IncomingService.IncomingServiceClient CreateIncomingClient()
    {
        return serviceProvider.GetRequiredService<IncomingService.IncomingServiceClient>();
    }

    public WarehouseService.WarehouseServiceClient CreateWarehouseClient()
    {
        return serviceProvider.GetRequiredService<WarehouseService.WarehouseServiceClient>();
    }
}