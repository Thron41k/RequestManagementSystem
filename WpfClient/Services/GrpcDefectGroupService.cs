using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using DefectGroup = RequestManagement.Common.Models.DefectGroup;
using Grpc.Core;
using RequestManagement.WpfClient.Services;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services;

public class GrpcDefectGroupService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IDefectGroupService
{
    public async Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectGroupClient();
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
        var client = clientFactory.CreateDefectGroupClient();
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
        var client = clientFactory.CreateDefectGroupClient();
        var result = await client.UpdateDefectGroupAsync(new UpdateDefectGroupRequest { DefectGroup = new RequestManagement.Server.Controllers.DefectGroup {Id = defectGroup.Id, Name = defectGroup.Name } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteDefectGroupAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateDefectGroupClient();
        var result = await client.DeleteDefectGroupAsync(new DeleteDefectGroupRequest { Id = id }, headers);
        return result.Success;
    }
}