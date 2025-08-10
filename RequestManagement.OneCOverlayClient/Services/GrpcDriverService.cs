using Grpc.Core;
using OneCOverlayClient.Services.Interfaces;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using Driver = RequestManagement.Common.Models.Driver;

namespace OneCOverlayClient.Services;

internal class GrpcDriverService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IDriverService
{
    public async Task<List<Driver>> GetAllDriversAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var response = await client.GetAllDriversAsync(new GetAllDriversRequest { Filter = filter }, headers);
        return response.Drivers.Select(driver => new Driver { Id = driver.Id, FullName = driver.FullName, ShortName = driver.ShortName, Position = driver.Position, Code = driver.Code}).ToList();
    }

    public async Task<int> CreateDriverAsync(Driver driver)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var result = await client.CreateDriverAsync(new CreateDriverRequest { Driver = new RequestManagement.Server.Controllers.Driver { FullName = driver.FullName, ShortName = driver.ShortName, Position = driver.Position, Code = driver.Code }}, headers);
        return result.Id;
    }

    public async Task<bool> UpdateDriverAsync(Driver driver)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var result = await client.UpdateDriverAsync(new UpdateDriverRequest { Driver = new RequestManagement.Server.Controllers.Driver { Id = driver.Id, FullName = driver.FullName, ShortName = driver.ShortName, Position = driver.Position, Code = driver.Code } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteDriverAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var result = await client.DeleteDriverAsync(new DeleteDriverRequest { Id = id }, headers);
        return result.Success;
    }

    public async Task<Driver> GetOrCreateDriverAsync(string requestFullName, string requestCode)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var result = await client.GetOrCreateDriverAsync(new GetOrCreateDriverRequest { FullName = requestFullName, Code = requestCode }, headers); 
        return new Driver
        {
            Id = result.Driver.Id, 
            FullName = result.Driver.FullName, 
            ShortName = result.Driver.ShortName, 
            Position = result.Driver.Position, 
            Code = result.Driver.Code
        };
    }
}