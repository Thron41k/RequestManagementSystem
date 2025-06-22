using RequestManagement.Common.Interfaces;
using Grpc.Core;
using RequestManagement.Server.Controllers;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

internal class GrpcDriverService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IDriverService
{
    public async Task<List<RequestManagement.Common.Models.Driver>> GetAllDriversAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDriverClient();
        var response = await client.GetAllDriversAsync(new GetAllDriversRequest { Filter = filter }, headers);
        return response.Drivers.Select(driver => new RequestManagement.Common.Models.Driver { Id = driver.Id, FullName = driver.FullName, ShortName = driver.ShortName, Position = driver.Position, Code = driver.Code}).ToList();
    }

    public async Task<int> CreateDriverAsync(RequestManagement.Common.Models.Driver driver)
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

    public async Task<bool> UpdateDriverAsync(RequestManagement.Common.Models.Driver driver)
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
}