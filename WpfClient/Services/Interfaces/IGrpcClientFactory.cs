using RequestManagement.Server.Controllers;

namespace WpfClient.Services.Interfaces;

public interface IGrpcClientFactory
{
    AuthService.AuthServiceClient CreateAuthClient();
    RequestService.RequestServiceClient CreateRequestClient();
    StockService.StockServiceClient CreateStockClient();
    ExpenseService.ExpenseServiceClient CreateExpenseClient();
}