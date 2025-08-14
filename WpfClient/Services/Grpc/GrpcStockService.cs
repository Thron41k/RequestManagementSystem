using System.Globalization;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using WpfClient.Models;

namespace RequestManagement.WpfClient.Services.Grpc;

internal class GrpcStockService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IStockService
{
    public async Task<List<Common.Models.Stock>> GetAllStocksAsync(
        int warehouseId,
        string filter = "",
        int initialQuantityFilterType = 0,
        double initialQuantity = 0,
        int receivedQuantityFilterType = 0,
        double receivedQuantity = 0,
        int consumedQuantityFilterType = 0,
        double consumedQuantity = 0,
        int finalQuantityFilterType = 0,
        double finalQuantity = 0
        )
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateStockClient();
        var response = await client.GetAllStockAsync(
            new GetAllStocksRequest
            {
                WarehouseId = warehouseId,
                Filter = filter,
                InitialQuantityFilterType = initialQuantityFilterType,
                InitialQuantity = initialQuantity,
                ReceivedQuantityFilterType = receivedQuantityFilterType,
                ReceivedQuantity = receivedQuantity,
                ConsumedQuantityFilterType = consumedQuantityFilterType,
                ConsumedQuantity = consumedQuantity,
                FinalQuantityFilterType = finalQuantityFilterType,
                FinalQuantity = finalQuantity
            }, headers);
        return response.Stocks.Select(stock => new Common.Models.Stock
        {
            Id = stock.Id,
            WarehouseId = stock.WarehouseId,
            NomenclatureId = stock.NomenclatureId,
            InitialQuantity = (decimal)stock.InitialQuantity,
            ReceivedQuantity = (decimal)stock.ReceivedQuantity,
            ConsumedQuantity = (decimal)stock.ConsumedQuantity,
            Nomenclature = new Common.Models.Nomenclature{Code = stock.Nomenclature.Code, Name = stock.Nomenclature.Name, Article = stock.Nomenclature.Article, UnitOfMeasure = stock.Nomenclature.UnitOfMeasure},
        }).ToList();
    }

    public async Task<bool> UploadMaterialsStockAsync(List<MaterialStock>? materials, int warehouseId, DateTime date)
    {
        if(materials == null) return false;
        if (materials.Count == 0) return false;
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateStockClient();
        var result = await client.UploadMaterialStockAsync(new UploadMaterialStockRequest { Materials = { materials.Select(material => new MaterialStockMessage { Name = material.ItemName, Article = material.Article, Code = material.Code, Unit = material.Unit, FinalBalance = material.FinalBalance}) }, WarehouseId = warehouseId, Date = date.ToString(CultureInfo.CurrentCulture) }, headers);
        return result.Success;
    }

    public async Task<int> CreateStockAsync(Common.Models.Stock stock)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateStockClient();
        var result = await client.CreateStockAsync(new CreateStockRequest { WarehouseId = stock.WarehouseId, NomenclatureId = stock.NomenclatureId, InitialQuantity = (double)stock.InitialQuantity }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateStockAsync(Common.Models.Stock stock)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateStockClient();
        var result = await client.UpdateStockAsync(new UpdateStockRequest { Id = stock.Id, NomenclatureId = stock.NomenclatureId, InitialQuantity = (double)stock.InitialQuantity }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteStockAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateStockClient();
        var result = await client.DeleteStockAsync(new DeleteStockRequest { Id = id }, headers);
        return result.Success;
    }
}