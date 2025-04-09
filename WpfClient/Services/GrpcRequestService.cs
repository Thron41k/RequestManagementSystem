using Grpc.Core;
using RequestManagement.Server.Controllers;

namespace WpfClient.Services;

public class GrpcRequestService
{
    private readonly RequestService.RequestServiceClient _client;
    private readonly AuthTokenStore _tokenStore;
    public GrpcRequestService(RequestService.RequestServiceClient client, AuthTokenStore tokenStore)
    {
        _client = client;
        _tokenStore = tokenStore;
    }

    public async Task<List<Equipment>> GetAllEquipmentAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }

        var response = await _client.GetAllEquipmentAsync(new GetAllEquipmentRequest{ Filter = filter }, headers);
        return response.Equipment.ToList();
    }

    public async Task CreateEquipmentAsync(CreateEquipmentRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }

        await _client.CreateEquipmentAsync(request, headers);
    }

    public async Task UpdateEquipmentAsync(UpdateEquipmentRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }

        await _client.UpdateEquipmentAsync(request, headers);
    }

    public async Task DeleteEquipmentAsync(DeleteEquipmentRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }

        await _client.DeleteEquipmentAsync(request, headers);
    }

    public async Task<List<Driver>> GetAllDriversAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        var response = await _client.GetAllDriversAsync(new GetAllDriversRequest { Filter = filter }, headers);
        return response.Drivers.ToList();
    }
    public async Task CreateDriverAsync(CreateDriverRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.CreateDriverAsync(request, headers);
    }

    public async Task UpdateDriverAsync(UpdateDriverRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.UpdateDriverAsync(request, headers);
    }

    public async Task DeleteDriverAsync(DeleteDriverRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.DeleteDriverAsync(request, headers);
    }
    public async Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        var response = await _client.GetAllDefectGroupsAsync(new GetAllDefectGroupsRequest { Filter = filter }, headers);
        return response.DefectGroup.ToList();
    }
    public async Task CreateDefectGroupAsync(CreateDefectGroupRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.CreateDefectGroupAsync(request, headers);
    }
    public async Task UpdateDefectGroupAsync(UpdateDefectGroupRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.UpdateDefectGroupAsync(request, headers);
    }
    public async Task DeleteDefectGroupAsync(DeleteDefectGroupRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.DeleteDefectGroupAsync(request, headers);
    }
    public async Task<List<Defect>> GetAllDefectsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        var response = await _client.GetAllDefectsAsync(new GetAllDefectsRequest { Filter = filter }, headers);
        return response.Defect.ToList();
    }
    public async Task CreateDefectAsync(CreateDefectRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.CreateDefectAsync(request, headers);
    }
    public async Task UpdateDefectAsync(UpdateDefectRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.UpdateDefectAsync(request, headers);
    }
    public async Task DeleteDefectAsync(DeleteDefectRequest request)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(_tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {_tokenStore.GetToken()}");
        }
        await _client.DeleteDefectAsync(request, headers);
    }
}