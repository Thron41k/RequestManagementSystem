using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using Defect = RequestManagement.Common.Models.Defect;
using DefectGroup = RequestManagement.Common.Models.DefectGroup;
using Grpc.Core;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

public class GrpcDefectService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IDefectService
{
    public async Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var response = await client.GetAllDefectGroupsAsync(new GetAllDefectGroupsRequest { Filter = filter }, headers);
        return response.DefectGroup.Select(defectGroup => new DefectGroup { Id = defectGroup.Id, Name = defectGroup.Name }).ToList();
    }

    public async Task<int> CreateDefectGroupAsync(DefectGroup defectGroup)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.CreateDefectGroupAsync(new CreateDefectGroupRequest { DefectGroup = new RequestManagement.Server.Controllers.DefectGroup { Name = defectGroup.Name } }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateDefectGroupAsync(DefectGroup defectGroup)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.UpdateDefectGroupAsync(new UpdateDefectGroupRequest { DefectGroup = new RequestManagement.Server.Controllers.DefectGroup { Name = defectGroup.Name } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteDefectGroupAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.DeleteDefectGroupAsync(new DeleteDefectGroupRequest { Id = id }, headers);
        return result.Success;
    }

    public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
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
        var client = clientFactory.CreateRequestClient();
        var result = await client.CreateDefectAsync(new CreateDefectRequest { Defect = new RequestManagement.Server.Controllers.Defect { Name = defect.Name, DefectGroupId = defect.DefectGroupId } }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateDefectAsync(Defect defect)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.UpdateDefectAsync(new UpdateDefectRequest { Defect = new RequestManagement.Server.Controllers.Defect { Name = defect.Name, DefectGroupId = defect.DefectGroupId } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteDefectAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateRequestClient();
        var result = await client.DeleteDefectAsync(new DeleteDefectRequest { Id = id }, headers);
        return result.Success;
    }
}