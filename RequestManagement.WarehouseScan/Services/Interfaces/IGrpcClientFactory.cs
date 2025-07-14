using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Server.Controllers;

namespace RequestManagement.WarehouseScan.Services.Interfaces
{
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
    }
}
