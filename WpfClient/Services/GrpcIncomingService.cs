using System.Globalization;
using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Incoming = RequestManagement.Common.Models.Incoming;
using Stock = RequestManagement.Common.Models.Stock;

namespace WpfClient.Services;

internal class GrpcIncomingService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IIncomingService
{
    public async Task<List<Incoming>> GetAllIncomingsAsync(string filter, int requestWarehouseId, string requestFromDate, string requestToDate)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var response = await client.GetAllIncomingsAsync(
            new GetAllIncomingsRequest
            {
                Filter = filter,
                WarehouseId = requestWarehouseId,
                FromDate = requestFromDate,
                ToDate = requestToDate
            }, headers);
        return response.Incoming.Select(incoming => new Incoming
        {
            Id = incoming.Id,
            StockId = incoming.Stock.Id,
            Stock = new Stock
            {
                Id = incoming.Stock.Id,
                WarehouseId = incoming.Stock.Warehouse.Id,
                Warehouse = new RequestManagement.Common.Models.Warehouse
                {
                    Id = incoming.Stock.Warehouse.Id,
                    Name = incoming.Stock.Warehouse.Name
                },
                NomenclatureId = incoming.Stock.Nomenclature.Id,
                Nomenclature = new RequestManagement.Common.Models.Nomenclature
                {
                    Id = incoming.Stock.Nomenclature.Id,
                    Name = incoming.Stock.Nomenclature.Name,
                    Code = incoming.Stock.Nomenclature.Code,
                    Article = incoming.Stock.Nomenclature.Article,
                    UnitOfMeasure = incoming.Stock.Nomenclature.UnitOfMeasure
                },
                InitialQuantity = (decimal)incoming.Stock.InitialQuantity,
                ReceivedQuantity = (decimal)incoming.Stock.ReceivedQuantity,
                ConsumedQuantity = (decimal)incoming.Stock.ConsumedQuantity
            },
            Date = Convert.ToDateTime(incoming.Date),
            Quantity = (decimal)incoming.Quantity
        }).ToList();
    }

    public async Task<Incoming> CreateIncomingAsync(Incoming incoming)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var result = await client.CreateIncomingAsync(
            new CreateIncomingRequest
            { 
                StockId = incoming.StockId,
                Date = incoming.Date.ToString(CultureInfo.CurrentCulture),
                Quantity = (double)incoming.Quantity
            }, headers);
        incoming.Id = result.Id;
        return incoming;
    }

    public async Task<bool> UpdateIncomingAsync(Incoming incoming)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var result = await client.UpdateIncomingAsync(
            new UpdateIncomingRequest
            {
                Id = incoming.Id,
                StockId = incoming.StockId,
                Date = incoming.Date.ToString(CultureInfo.CurrentCulture),
                Quantity = (double)incoming.Quantity
            }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteIncomingAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var result = await client.DeleteIncomingAsync(new DeleteIncomingRequest { Id = id }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteIncomingsAsync(List<int> requestIds)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var result = await client.DeleteIncomingsAsync(new DeleteIncomingsRequest {Id = { requestIds } }, headers);
        return result.Success;
    }
}