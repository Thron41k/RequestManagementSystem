using System.Globalization;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Converters;
using RequestManagement.WpfClient.Services.Interfaces;
using Incoming = RequestManagement.Common.Models.Incoming;

namespace RequestManagement.WpfClient.Services;

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
        return IncomingConverter.FromGrpc(response).ToList();
    }

    public async Task<bool> UploadIncomingsAsync(MaterialIncoming incoming)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateIncomingClient();
        var newList = new List<IncomingMaterialItem>();
        foreach (var item in incoming.Items)
        {
            var newItem = new IncomingMaterialItem
            {
                RegistratorNumber = item.RegistratorNumber,
                RegistratorType = item.RegistratorType,
                RegistratorDate = item.RegistratorDate,
                ReceiptOrderNumber = item.ReceiptOrderNumber ?? "",
                ReceiptOrderDate = item.ReceiptOrderDate ?? "",
                ApplicationNumber = item.ApplicationNumber ?? "",
                ApplicationDate = item.ApplicationDate ?? "",
                ApplicationResponsibleName = item.ApplicationResponsibleName ?? "",
                ApplicationEquipmentName = item.ApplicationEquipmentName ?? "",
                ApplicationEquipmentCode = item.ApplicationEquipmentCode ?? "",
                InWarehouseName = item.InWarehouseName ?? "",
                InWarehouseCode = item.InWarehouseCode ?? ""
            };
            foreach (var i in item.Items)
            {
                newItem.Items.Add(new IncomingMaterialStockMessage
                {
                    Name = i.ItemName,
                    Code = i.Code,
                    Article = i.Article ?? "",
                    Unit = i.Unit,
                    FinalBalance = i.FinalBalance
                });
            }
            newList.Add(newItem);
        }
        var result = await client.UploadMaterialIncomingAsync(new UploadMaterialIncomingRequest
        {
            WarehouseName = incoming.WarehouseName,
            Items = { newList }
        }, headers);
        return result.Success;
    }

    public Task<Incoming> FindIncomingByIdAsync(int id)
    {
        throw new NotImplementedException();
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