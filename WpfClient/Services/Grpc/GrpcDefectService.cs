using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using Defect = RequestManagement.Common.Models.Defect;

namespace RequestManagement.WpfClient.Services.Grpc;

public class GrpcDefectService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IDefectService
{
    public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectClient();
        var response = await client.GetAllDefectsAsync(new GetAllDefectsRequest { Filter = filter }, headers);
        return response.Defect.Select(defect => new Defect { Id = defect.Id, Name = defect.Name, DefectGroupId = defect.DefectGroupId }).ToList();
    }

    public async Task<int> CreateDefectAsync(Defect defect)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectClient();
        var result = await client.CreateDefectAsync(new CreateDefectRequest { Defect = new Server.Controllers.Defect { Name = defect.Name, DefectGroupId = defect.DefectGroupId } }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateDefectAsync(Defect defect)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectClient();
        var result = await client.UpdateDefectAsync(new UpdateDefectRequest { Defect = new Server.Controllers.Defect { Id = defect.Id, Name = defect.Name, DefectGroupId = defect.DefectGroupId } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteDefectAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectClient();
        var result = await client.DeleteDefectAsync(new DeleteDefectRequest { Id = id }, headers);
        return result.Success;
    }
}