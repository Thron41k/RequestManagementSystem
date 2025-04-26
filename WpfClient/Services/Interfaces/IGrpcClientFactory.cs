using AuthService = RequestManagement.Server.Controllers.AuthService;
using ExpenseService = RequestManagement.Server.Controllers.ExpenseService;
using RequestService = RequestManagement.Server.Controllers.RequestService;
using StockService = RequestManagement.Server.Controllers.StockService;
using IncomingService = RequestManagement.Server.Controllers.IncomingService;
using WarehouseService = RequestManagement.Server.Controllers.WarehouseService;

namespace WpfClient.Services.Interfaces;

public interface IGrpcClientFactory
{
    AuthService.AuthServiceClient CreateAuthClient();
    RequestService.RequestServiceClient CreateRequestClient();
    StockService.StockServiceClient CreateStockClient();
    ExpenseService.ExpenseServiceClient CreateExpenseClient();
    IncomingService.IncomingServiceClient CreateIncomingClient();
    WarehouseService.WarehouseServiceClient CreateWarehouseClient();
}