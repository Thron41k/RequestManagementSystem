using System.Globalization;
using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Common.Models;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Expense = RequestManagement.Common.Models.Expense;
using MaterialExpense = RequestManagement.Common.Models.MaterialExpense;
using Stock = RequestManagement.Common.Models.Stock;

namespace WpfClient.Services;

internal class GrpcExpenseService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IExpenseService
{
    public async Task<List<Expense>> GetAllExpensesAsync(string filter, int requestWarehouseId, int requestEquipmentId, int requestDriverId,
        int requestDefectId, string requestFromDate, string requestToDate)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var response = await client.GetAllExpensesAsync(
            new GetAllExpensesRequest
            {
                Filter = filter,
                WarehouseId = requestWarehouseId,
                EquipmentId = requestEquipmentId,
                DriverId = requestDriverId,
                DefectId = requestDefectId,
                FromDate = requestFromDate,
                ToDate = requestToDate
            }, headers);
        return response.Expenses.Select(expense => new Expense
        {
            Id = expense.Id,
            Code = expense.Code,
            StockId = expense.Stock.Id,
            Stock = new Stock
            {
                Id = expense.Stock.Id,
                WarehouseId = expense.Stock.Warehouse.Id,
                Warehouse = new RequestManagement.Common.Models.Warehouse
                {
                    Id = expense.Stock.Warehouse.Id,
                    Name = expense.Stock.Warehouse.Name
                },
                NomenclatureId = expense.Stock.Nomenclature.Id,
                Nomenclature = new RequestManagement.Common.Models.Nomenclature
                {
                    Id = expense.Stock.Nomenclature.Id,
                    Name = expense.Stock.Nomenclature.Name,
                    Code = expense.Stock.Nomenclature.Code,
                    Article = expense.Stock.Nomenclature.Article,
                    UnitOfMeasure = expense.Stock.Nomenclature.UnitOfMeasure
                },
                InitialQuantity = (decimal)expense.Stock.InitialQuantity,
                ReceivedQuantity = (decimal)expense.Stock.ReceivedQuantity,
                ConsumedQuantity = (decimal)expense.Stock.ConsumedQuantity
            },
            EquipmentId = expense.Equipment.Id,
            Equipment = new RequestManagement.Common.Models.Equipment
            {
                Id = expense.Equipment.Id,
                Code = expense.Equipment.Code,
                Name = expense.Equipment.Name,
                StateNumber = expense.Equipment.LicensePlate
            },
            DriverId = expense.Driver.Id,
            Driver = new RequestManagement.Common.Models.Driver
            {
                Id = expense.Driver.Id,
                Code = expense.Driver.Code,
                FullName = expense.Driver.FullName,
                ShortName = expense.Driver.ShortName,
                Position = expense.Driver.Position
            },
            DefectId = expense.Defect.Id,
            Defect = new RequestManagement.Common.Models.Defect
            {
                Id = expense.Defect.Id,
                Name = expense.Defect.Name,
                DefectGroupId = expense.Defect.DefectGroup.Id,
                DefectGroup = new RequestManagement.Common.Models.DefectGroup
                {
                    Id = expense.Defect.DefectGroup.Id,
                    Name = expense.Defect.DefectGroup.Name
                }
            },
            Date = Convert.ToDateTime(expense.Date),
            Quantity = (decimal)expense.Quantity,
            Term = expense.Term
        }).ToList();
    }

    public async Task<bool> UploadMaterialsExpenseAsync(List<MaterialExpense>? materials, int warehouseId)
    {
        if(materials == null) return false;
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.UploadMaterialExpenseAsync(
            new UploadMaterialExpenseRequest
            {
                MaterialExpenses =
                {
                    materials.Select(
                        material => new RequestManagement.Server.Controllers.MaterialExpense
                        {
                            Number = material.Number,
                            DriverFullName = material.DriverFullName,
                            DriverCode = material.DriverCode,
                            EquipmentName = material.EquipmentName,
                            Date = material.Date.ToString(CultureInfo.CurrentCulture),
                            EquipmentCode = material.EquipmentCode,
                            NomenclatureName = material.NomenclatureName,
                            NomenclatureCode = material.NomenclatureCode,
                            NomenlatureUnitOfMeasure = material.NomenlatureUnitOfMeasure,
                            Quantity = (double)material.Quantity,
                            NomenclatureArticle = material.NomenclatureArticle,
                        })
                },WarehouseId = warehouseId
            }, headers);
        return result.Success;
    }

    public async Task<Expense> CreateExpenseAsync(Expense expense)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.CreateExpenseAsync(
            new CreateExpenseRequest
            { 
                StockId = expense.StockId,
                EquipmentId = expense.EquipmentId,
                DriverId = expense.DriverId,
                DefectId = expense.DefectId,
                Date = expense.Date.ToString(CultureInfo.CurrentCulture),
                Quantity = (double)expense.Quantity,
                Term = expense.Term ?? 0
            }, headers);
        expense.Id = result.Id;
        return expense;
    }

    public async Task<bool> UpdateExpenseAsync(Expense expense)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.UpdateExpenseAsync(
            new UpdateExpenseRequest
            {
                Id = expense.Id,
                StockId = expense.StockId,
                EquipmentId = expense.EquipmentId,
                DriverId = expense.DriverId,
                DefectId = expense.DefectId,
                Date = expense.Date.ToString(CultureInfo.CurrentCulture),
                Quantity = (double)expense.Quantity,
                Term = expense.Term ?? 0
            }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteExpenseAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.DeleteExpenseAsync(new DeleteExpenseRequest { Id = id }, headers);
        return result.Success;
    }

    public async Task<UserLastSelection?> GetUserLastSelectionAsync(int userId)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.GetLastSelectionAsync(new GetLastSelectionRequest { NomenclatureId = userId }, headers);
        return new UserLastSelection
        {
            Driver = new RequestManagement.Common.Models.Driver { Id = result.Driver.Id, FullName = result.Driver.FullName, ShortName = result.Driver.ShortName, Position = result.Driver.Position },
            Equipment = new RequestManagement.Common.Models.Equipment { Id = result.Equipment.Id, Name = result.Equipment.Name, StateNumber = result.Equipment.LicensePlate },
            
        };
    }

    public Task SaveUserLastSelectionAsync(int userId, int? driverId, int? equipmentId)
    {
        throw new NotImplementedException();
    }

    public async Task<NomenclatureDefectMapping?> GetLastNomenclatureDefectMappingAsync(int userId, int nomenclatureId)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.GetNomenclatureMapingAsync(new GetNomenclatureMapingRequest { NomenclatureId = nomenclatureId }, headers);
        var mapping = new NomenclatureDefectMapping();
        if (result is { Defect: not null })
        {
            mapping.Defect = new RequestManagement.Common.Models.Defect
                { Id = result.Defect.Id, Name = result.Defect.Name };
            mapping.Term = result.Term;
        }
        return mapping;
    }

    public Task SaveNomenclatureDefectMappingAsync(int userId, int stockId, int defectId, int term)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteExpensesAsync(List<int> requestIds)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateExpenseClient();
        var result = await client.DeleteExpensesAsync(new DeleteExpensesRequest {Id = { requestIds } }, headers);
        return result.Success;
    }
}