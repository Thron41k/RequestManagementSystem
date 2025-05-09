// ServiceConfigurator.cs
using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using WpfClient.Services;
using WpfClient.Services.ExcelTemplate;
using WpfClient.Services.Interfaces;
using WpfClient.ViewModels;
using WpfClient.Views;

namespace WpfClient;

public class ServiceConfigurator
{
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        // gRPC клиенты
        serviceCollection.AddGrpcClient<AuthService.AuthServiceClient>(o => {o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<RequestService.RequestServiceClient>(o => {o.Address = new Uri("http://localhost:5001");});
        serviceCollection.AddGrpcClient<StockService.StockServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<ExpenseService.ExpenseServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<IncomingService.IncomingServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<WarehouseService.WarehouseServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });
        serviceCollection.AddGrpcClient<CommissionsService.CommissionsServiceClient>(o => { o.Address = new Uri("http://localhost:5001"); });

        // Сервисы и ViewModel'ы
        serviceCollection.AddSingleton<IPrinterService, PrinterService>();
        serviceCollection.AddSingleton<AuthTokenStore>();
        serviceCollection.AddSingleton<IMessageBus, MessageBusService>();
        serviceCollection.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
        serviceCollection.AddSingleton<IExcelReaderService, ExcelReaderService>();
        serviceCollection.AddSingleton<IExcelWriterService, ExcelWriterService>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, ActPartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, LimitPartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, DefectPartsTemplate>();
        serviceCollection.AddSingleton<IFileSaveDialogService, FileSaveDialogService>();
        serviceCollection.AddSingleton<IExcelPrintService, ExcelPrintService>();

        serviceCollection.AddScoped<GrpcAuthService>();
        serviceCollection.AddScoped<LoginViewModel>();
        serviceCollection.AddScoped<EquipmentViewModel>();
        serviceCollection.AddScoped<MainMenuViewModel>();
        serviceCollection.AddScoped<DriverViewModel>();
        serviceCollection.AddScoped<DefectGroupViewModel>();
        serviceCollection.AddScoped<DefectViewModel>();
        serviceCollection.AddScoped<WarehouseViewModel>();
        serviceCollection.AddScoped<NomenclatureViewModel>();
        serviceCollection.AddScoped<StockViewModel>();
        serviceCollection.AddScoped<ExpenseViewModel>();
        serviceCollection.AddScoped<ExpenseListViewModel>();
        serviceCollection.AddScoped<IncomingListViewModel>();
        serviceCollection.AddScoped<StartDataLoadViewModel>();
        serviceCollection.AddScoped<ExpenseDataLoadViewModel>();
        serviceCollection.AddScoped<CommissionsViewModel>();
        serviceCollection.AddScoped<PrintReportViewModel>();
        serviceCollection.AddScoped<IEquipmentService, GrpcEquipmentService>();
        serviceCollection.AddScoped<IDriverService, GrpcDriverService>();
        serviceCollection.AddScoped<IDefectService, GrpcDefectService>();
        serviceCollection.AddScoped<IWarehouseService, GrpcWarehouseService>();
        serviceCollection.AddScoped<INomenclatureService, GrpcNomenclatureService>();
        serviceCollection.AddScoped<IStockService, GrpcStockService>();
        serviceCollection.AddScoped<IExpenseService, GrpcExpenseService>();
        serviceCollection.AddScoped<IIncomingService, GrpcIncomingService>();
        serviceCollection.AddScoped<IWarehouseService, GrpcWarehouseService>();
        serviceCollection.AddScoped<ICommissionsService, GrpcCommissionsService>();

        // Представления
        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddTransient<DriverView>();
        serviceCollection.AddTransient<MainMenu>();
        serviceCollection.AddTransient<EquipmentView>();
        serviceCollection.AddTransient<DefectGroupView>();
        serviceCollection.AddTransient<DefectView>();
        serviceCollection.AddTransient<WarehouseView>();
        serviceCollection.AddTransient<NomenclatureView>();
        serviceCollection.AddTransient<StockView>();
        serviceCollection.AddTransient<ExpenseView>();
        serviceCollection.AddTransient<ExpenseListView>();
        serviceCollection.AddTransient<IncomingListView>();
        serviceCollection.AddTransient<StartDataLoadView>();
        serviceCollection.AddTransient<ExpenseDataLoadView>();
        serviceCollection.AddTransient<CommissionsView>();
        serviceCollection.AddTransient<PrintReportView>();

        return serviceCollection.BuildServiceProvider();
    }
}