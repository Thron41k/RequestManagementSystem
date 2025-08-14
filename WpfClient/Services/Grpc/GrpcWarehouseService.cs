using System.Globalization;
using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using Warehouse = RequestManagement.Common.Models.Warehouse;

namespace RequestManagement.WpfClient.Services.Grpc;

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
        return response.Warehouse.Select(warehouse => new Warehouse
        {
            Id = warehouse.Id, 
            Name = warehouse.Name, 
            LastUpdated = DateTime.Parse(warehouse.LastUpdated),
            Code = warehouse.Code,
            FinanciallyResponsiblePersonId = warehouse.FinanciallyResponsiblePersonId,
            FinanciallyResponsiblePerson = warehouse.FinanciallyResponsiblePerson != null ? new Common.Models.Driver
            {
                Id = warehouse.FinanciallyResponsiblePerson.Id,
                FullName = warehouse.FinanciallyResponsiblePerson.FullName,
                ShortName = warehouse.FinanciallyResponsiblePerson.ShortName,
                Position = warehouse.FinanciallyResponsiblePerson.Position,
                Code = warehouse.FinanciallyResponsiblePerson.Code
            } : null
        }).ToList();
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
        return new Warehouse
        {
            Id = response.Warehouse.Id, 
            Name = response.Warehouse.Name, 
            LastUpdated = DateTime.Parse(response.Warehouse.LastUpdated),
            Code = response.Warehouse.Code,
            FinanciallyResponsiblePersonId = response.Warehouse.FinanciallyResponsiblePersonId,
            FinanciallyResponsiblePerson = response.Warehouse.FinanciallyResponsiblePerson != null ? new Common.Models.Driver
            {
                Id = response.Warehouse.FinanciallyResponsiblePerson.Id,
                FullName = response.Warehouse.FinanciallyResponsiblePerson.FullName,
                ShortName = response.Warehouse.FinanciallyResponsiblePerson.ShortName,
                Position = response.Warehouse.FinanciallyResponsiblePerson.Position,
                Code = response.Warehouse.FinanciallyResponsiblePerson.Code
            } : null
        };
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
            var result = await client.CreateWarehouseAsync(new CreateWarehouseRequest
            {
                Warehouse = new Server.Controllers.Warehouse
                {
                    Name = warehouse.Name, 
                    LastUpdated = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    Code = warehouse.Code,
                    FinanciallyResponsiblePersonId = warehouse.FinanciallyResponsiblePersonId ?? 0
                }
            }, headers);
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
        var result = await client.UpdateWarehouseAsync(new UpdateWarehouseRequest
        {
            Warehouse = new Server.Controllers.Warehouse
            {
                Id = warehouse.Id, 
                Name = warehouse.Name, 
                LastUpdated = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                Code = warehouse.Code,
                FinanciallyResponsiblePersonId = warehouse.FinanciallyResponsiblePersonId ?? 0
            }
        }, headers);
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