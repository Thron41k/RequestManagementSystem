using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.Services.Grpc;

internal class GrpcNomenclatureService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : INomenclatureService
{
    public async Task<List<Common.Models.Nomenclature>> GetAllNomenclaturesAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateNomenclatureClient();
        var response = await client.GetAllNomenclaturesAsync(new GetAllNomenclaturesRequest { Filter = filter }, headers);
        return response.Nomenclature.Select(nomenclature => new Common.Models.Nomenclature { Id = nomenclature.Id, Name = nomenclature.Name,Code = nomenclature.Code,Article = nomenclature.Article,UnitOfMeasure = nomenclature.UnitOfMeasure}).ToList();
    }

    public async Task<int> CreateNomenclatureAsync(Common.Models.Nomenclature nomenclature)
    {
        try
        {
            var headers = new Metadata();
            if (!string.IsNullOrEmpty(tokenStore.GetToken()))
            {
                headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
            }
            var client = clientFactory.CreateNomenclatureClient();
            var result = await client.CreateNomenclatureAsync(new CreateNomenclatureRequest { Nomenclature = new Nomenclature { Name = nomenclature.Name,Code = nomenclature.Code,Article = nomenclature.Article,UnitOfMeasure = nomenclature.UnitOfMeasure } }, headers);
            return result.Id;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return -1;
        }
    }

    public async Task<bool> UpdateNomenclatureAsync(Common.Models.Nomenclature nomenclature)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateNomenclatureClient();
        var result = await client.UpdateNomenclatureAsync(new UpdateNomenclatureRequest { Nomenclature = new Nomenclature { Id = nomenclature.Id, Name = nomenclature.Name,Code = nomenclature.Code,Article = nomenclature.Article,UnitOfMeasure = nomenclature.UnitOfMeasure } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteNomenclatureAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateNomenclatureClient();
        var result = await client.DeleteNomenclatureAsync(new DeleteNomenclatureRequest { Id = id }, headers);
        return result.Success;
    }
}