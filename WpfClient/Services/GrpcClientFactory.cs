using Microsoft.Extensions.DependencyInjection;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.Services;

public class GrpcClientFactory(IServiceProvider serviceProvider) : IGrpcClientFactory
{
    public AuthService.AuthServiceClient CreateAuthClient()
    {
        return serviceProvider.GetRequiredService<AuthService.AuthServiceClient>();
    }

    public NomenclatureService.NomenclatureServiceClient CreateNomenclatureClient()
    {
        return serviceProvider.GetRequiredService<NomenclatureService.NomenclatureServiceClient>();
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

    public CommissionsService.CommissionsServiceClient CreateCommissionsClient()
    {
        return serviceProvider.GetRequiredService<CommissionsService.CommissionsServiceClient>();
    }

    public NomenclatureAnalogService.NomenclatureAnalogServiceClient CreateNomenclatureAnalogClient()
    {
        return serviceProvider.GetRequiredService<NomenclatureAnalogService.NomenclatureAnalogServiceClient>();
    }

    public EquipmentService.EquipmentServiceClient CreateEquipmentClient()
    {
        return serviceProvider.GetRequiredService<EquipmentService.EquipmentServiceClient>();
    }

    public DriverService.DriverServiceClient CreateDriverClient()
    {
        return serviceProvider.GetRequiredService<DriverService.DriverServiceClient>();
    }

    public DefectService.DefectServiceClient CreateDefectClient()
    {
        return serviceProvider.GetRequiredService<DefectService.DefectServiceClient>();
    }

    public DefectGroupService.DefectGroupServiceClient CreateDefectGroupClient()
    {
        return serviceProvider.GetRequiredService<DefectGroupService.DefectGroupServiceClient>();
    }

    public EquipmentGroupService.EquipmentGroupServiceClient CreateEquipmentGroupClient()
    {
        return serviceProvider.GetRequiredService<EquipmentGroupService.EquipmentGroupServiceClient>();
    }

    public SparePartsOwnershipService.SparePartsOwnershipServiceClient CreateSparePartsOwnershipClient()
    {
        return serviceProvider.GetRequiredService<SparePartsOwnershipService.SparePartsOwnershipServiceClient>();
    }

    public MaterialsInUseService.MaterialsInUseServiceClient CreateMaterialsInUseClient()
    {
        return serviceProvider.GetRequiredService<MaterialsInUseService.MaterialsInUseServiceClient>();
    }

    public ReasonsForWritingOffMaterialsFromOperationService.ReasonsForWritingOffMaterialsFromOperationServiceClient CreateReasonsForWritingOffMaterialsFromOperationClient()
    {
        return serviceProvider.GetRequiredService<ReasonsForWritingOffMaterialsFromOperationService.ReasonsForWritingOffMaterialsFromOperationServiceClient>();
    }
}