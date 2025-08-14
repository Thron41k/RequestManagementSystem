// ServiceConfigurator.cs

using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services;
using RequestManagement.WpfClient.Services.ExcelTemplate;
using RequestManagement.WpfClient.Services.Grpc;
using RequestManagement.WpfClient.Services.Interfaces;
using RequestManagement.WpfClient.ViewModels;
using RequestManagement.WpfClient.Views;
using IncomingListViewModel = RequestManagement.WpfClient.ViewModels.IncomingListViewModel;

namespace RequestManagement.WpfClient;

public class ServiceConfigurator
{
    public static IServiceProvider ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddMessageHandlers();

        // gRPC клиенты
        serviceCollection.AddGrpcClient<AuthService.AuthServiceClient>(o => {o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<NomenclatureService.NomenclatureServiceClient>(o => {o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<StockService.StockServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<ExpenseService.ExpenseServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<IncomingService.IncomingServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<WarehouseService.WarehouseServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<CommissionsService.CommissionsServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<NomenclatureAnalogService.NomenclatureAnalogServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<EquipmentService.EquipmentServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<DriverService.DriverServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<DefectService.DefectServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<DefectGroupService.DefectGroupServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<EquipmentGroupService.EquipmentGroupServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<SparePartsOwnershipService.SparePartsOwnershipServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<MaterialsInUseService.MaterialsInUseServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });
        serviceCollection.AddGrpcClient<ReasonsForWritingOffMaterialsFromOperationService.ReasonsForWritingOffMaterialsFromOperationServiceClient>(o => { o.Address = new Uri($"http://{Vars.Server}:{Vars.Port}"); });

        // Сервисы и ViewModel'ы
        serviceCollection.AddSingleton<IPrinterService, PrinterService>();
        serviceCollection.AddSingleton<AuthTokenStore>();
        serviceCollection.AddSingleton<IGrpcClientFactory, GrpcClientFactory>();
        serviceCollection.AddSingleton<IExcelReaderService, ExcelReaderService>();
        serviceCollection.AddSingleton<IExcelWriterService, ExcelWriterService>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, ActPartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, LimitPartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, DefectPartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, Mb7PartsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, ConsumablesTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, OperationsTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, RequisitionInvoiceTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, MovingBetweenWarehousesTemplate>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, MaterialsInUseOffTemplateV1>();
        serviceCollection.AddSingleton<IExcelTemplateWriter, MaterialsInUseOffTemplateV2>();
        serviceCollection.AddSingleton<IFileSaveDialogService, FileSaveDialogService>();
        serviceCollection.AddSingleton<IExcelPrintService, ExcelPrintService>();

        serviceCollection.AddScoped<LoginViewModel>();
        serviceCollection.AddScoped<EquipmentViewModel>();
        serviceCollection.AddScoped<MainMenuViewModel>();
        serviceCollection.AddScoped<DriverViewModel>();
        serviceCollection.AddScoped<DefectGroupViewModel>();
        serviceCollection.AddScoped<EquipmentGroupViewModel>();
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
        serviceCollection.AddScoped<SparePartsAnalogsViewModel>();
        serviceCollection.AddScoped<IncomingDataLoadViewModel>();
        serviceCollection.AddScoped<LabelCountSelectorViewModel>();
        serviceCollection.AddScoped<LabelPrintListViewModel>();
        serviceCollection.AddScoped<SparePartsOwnershipViewModel>();
        serviceCollection.AddScoped<MaterialsInUseLoadViewModel>();
        serviceCollection.AddScoped<MaterialInUseListViewModel>();
        serviceCollection.AddScoped<AddMaterialsInUseToOffViewModel>();
        serviceCollection.AddScoped<ReasonsForWritingOffMaterialsFromOperationViewModel>();

        serviceCollection.AddScoped<GrpcAuthService>();
        serviceCollection.AddScoped<IEquipmentService, GrpcEquipmentService>();
        serviceCollection.AddScoped<IDriverService, GrpcDriverService>();
        serviceCollection.AddScoped<IDefectService, GrpcDefectService>();
        serviceCollection.AddScoped<IDefectGroupService, GrpcDefectGroupService>();
        serviceCollection.AddScoped<IWarehouseService, GrpcWarehouseService>();
        serviceCollection.AddScoped<INomenclatureService, GrpcNomenclatureService>();
        serviceCollection.AddScoped<IStockService, GrpcStockService>();
        serviceCollection.AddScoped<IExpenseService, GrpcExpenseService>();
        serviceCollection.AddScoped<IIncomingService, GrpcIncomingService>();
        serviceCollection.AddScoped<IWarehouseService, GrpcWarehouseService>();
        serviceCollection.AddScoped<ICommissionsService, GrpcCommissionsService>();
        serviceCollection.AddScoped<IEquipmentGroupService, GrpcEquipmentGroupService>();
        serviceCollection.AddScoped<INomenclatureAnalogService, GrpcNomenclatureAnalogService>();
        serviceCollection.AddScoped<ISparePartsOwnershipService, GrpcSparePartsOwnershipService>();
        serviceCollection.AddScoped<IMaterialsInUseService, GrpcMaterialsInUseService>();
        serviceCollection.AddScoped<IReasonsForWritingOffMaterialsFromOperationService, GrpcReasonsForWritingOffMaterialsFromOperationService>();

        // Представления
        serviceCollection.AddTransient<MainWindow>();
        serviceCollection.AddTransient<DriverView>();
        serviceCollection.AddTransient<MainMenu>();
        serviceCollection.AddTransient<EquipmentView>();
        serviceCollection.AddTransient<EquipmentGroupView>();
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
        serviceCollection.AddTransient<SparePartsAnalogsView>();
        serviceCollection.AddTransient<IncomingDataLoadView>();
        serviceCollection.AddTransient<LabelCountSelectorView>();
        serviceCollection.AddTransient<LabelPrintListView>();
        serviceCollection.AddTransient<SparePartsOwnershipView>();
        serviceCollection.AddTransient<MaterialsInUseLoadView>();
        serviceCollection.AddTransient<MaterialInUseListView>();
        serviceCollection.AddTransient<AddMaterialsInUseToOffView>();
        serviceCollection.AddTransient<ReasonsForWritingOffMaterialsFromOperationView>();

        return serviceCollection.BuildServiceProvider();
    }
}