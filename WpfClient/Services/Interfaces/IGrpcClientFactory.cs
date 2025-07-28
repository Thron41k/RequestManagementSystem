using RequestManagement.Server.Controllers;

namespace RequestManagement.WpfClient.Services.Interfaces;

public interface IGrpcClientFactory
{
    AuthService.AuthServiceClient CreateAuthClient();
    NomenclatureService.NomenclatureServiceClient CreateNomenclatureClient();
    StockService.StockServiceClient CreateStockClient();
    ExpenseService.ExpenseServiceClient CreateExpenseClient();
    IncomingService.IncomingServiceClient CreateIncomingClient();
    WarehouseService.WarehouseServiceClient CreateWarehouseClient();
    CommissionsService.CommissionsServiceClient CreateCommissionsClient();
    NomenclatureAnalogService.NomenclatureAnalogServiceClient CreateNomenclatureAnalogClient();
    EquipmentService.EquipmentServiceClient CreateEquipmentClient();
    DriverService.DriverServiceClient CreateDriverClient();
    DefectService.DefectServiceClient CreateDefectClient();
    DefectGroupService.DefectGroupServiceClient CreateDefectGroupClient();
    EquipmentGroupService.EquipmentGroupServiceClient CreateEquipmentGroupClient();
    SparePartsOwnershipService.SparePartsOwnershipServiceClient CreateSparePartsOwnershipClient();
}