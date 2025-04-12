using System.Globalization;
using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Stock = RequestManagement.Common.Models.Stock;

namespace WpfClient.Services;

internal class GrpcExpenseService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IExpenseService
{
    public async Task<List<RequestManagement.Common.Models.Expense>> GetAllExpensesAsync(string filter = "")
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
                Filter = filter
            }, headers);
        return response.Expenses.Select(expense => new RequestManagement.Common.Models.Expense
        {
            Id = expense.Id,
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
                Name = expense.Equipment.Name,
                StateNumber = expense.Equipment.LicensePlate
            },
            DriverId = expense.Driver.Id,
            Driver = new RequestManagement.Common.Models.Driver
            {
                Id = expense.Driver.Id,
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
            Quantity = (decimal)expense.Quantity
        }).ToList();
    }

    public async Task<int> CreateExpenseAsync(RequestManagement.Common.Models.Expense expense)
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
                Quantity = (double)expense.Quantity
            }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateExpenseAsync(RequestManagement.Common.Models.Expense expense)
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
                StockId = expense.StockId,
                EquipmentId = expense.EquipmentId,
                DriverId = expense.DriverId,
                DefectId = expense.DefectId,
                Date = expense.Date.ToString(CultureInfo.CurrentCulture),
                Quantity = (double)expense.Quantity
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
}