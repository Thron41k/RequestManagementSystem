using System.Globalization;
using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;
using Warehouse = RequestManagement.Common.Models.Warehouse;

namespace WpfClient.Services;

internal class GrpcWarehouseService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IWarehouseService
{
    public async Task<List<Warehouse>> GetAllWarehousesAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateWarehouseClient();
        var response = await client.GetAllWarehousesAsync(new GetAllWarehousesRequest { Filter = filter }, headers);
        return response.Warehouse.Select(warehouse => new Warehouse { Id = warehouse.Id, Name = warehouse.Name, LastUpdated = DateTime.Parse(warehouse.LastUpdated) }).ToList();
    }

    public async Task<Warehouse> GetOrCreateWarehousesAsync(string filter)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateWarehouseClient();
        var response = await client.GetOrCreateWarehouseAsync(new GetOrCreateWarehouseRequest { Filter = filter }, headers);
        return new Warehouse { Id = response.Warehouse.Id, Name = response.Warehouse.Name, LastUpdated = DateTime.Parse(response.Warehouse.LastUpdated) };
    }

    public async Task<int> CreateWarehouseAsync(Warehouse warehouse)
    {
        try
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateWarehouseClient();
            var result = await client.CreateWarehouseAsync(new CreateWarehouseRequest { Warehouse = new RequestManagement.Server.Controllers.Warehouse { Name = warehouse.Name, LastUpdated = DateTime.Now.ToString(CultureInfo.CurrentCulture) } }, headers);
            return result.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }

    public async Task<bool> UpdateWarehouseAsync(Warehouse warehouse)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateWarehouseClient();
        var result = await client.UpdateWarehouseAsync(new UpdateWarehouseRequest { Warehouse = new RequestManagement.Server.Controllers.Warehouse { Id = warehouse.Id, Name = warehouse.Name, LastUpdated = DateTime.Now.ToString(CultureInfo.CurrentCulture) } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteWarehouseAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateWarehouseClient();
        var result = await client.DeleteWarehouseAsync(new DeleteWarehouseRequest { Id = id }, headers);
        return result.Success;
    }
}