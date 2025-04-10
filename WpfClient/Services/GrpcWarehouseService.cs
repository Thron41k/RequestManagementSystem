using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

internal class GrpcWarehouseService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IWarehouseService
{
    public async Task<List<RequestManagement.Common.Models.Warehouse>> GetAllWarehousesAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var response = await client.GetAllWarehousesAsync(new GetAllWarehousesRequest { Filter = filter }, headers);
        return response.Warehouse.Select(warehouse => new RequestManagement.Common.Models.Warehouse { Id = warehouse.Id, Name = warehouse.Name}).ToList();
    }

    public async Task<int> CreateWarehouseAsync(RequestManagement.Common.Models.Warehouse warehouse)
    {
        try
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateRequestClient();
            var result = await client.CreateWarehouseAsync(new CreateWarehouseRequest { Warehouse = new RequestManagement.Server.Controllers.Warehouse { Name = warehouse.Name } }, headers);
            return result.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }

    public async Task<bool> UpdateWarehouseAsync(RequestManagement.Common.Models.Warehouse warehouse)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.UpdateWarehouseAsync(new UpdateWarehouseRequest { Warehouse = new RequestManagement.Server.Controllers.Warehouse { Id = warehouse.Id, Name = warehouse.Name } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.DeleteWarehouseAsync(new DeleteWarehouseRequest { Id = id }, headers);
        return result.Success;
    }
}