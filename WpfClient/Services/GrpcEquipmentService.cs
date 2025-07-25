using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;

namespace RequestManagement.WpfClient.Services;

public class GrpcEquipmentService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore)
    : IEquipmentService
{
    public async Task<int> CreateEquipmentAsync(RequestManagement.Common.Models.Equipment equipment)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var request = new CreateEquipmentRequest
        {
            Name = equipment.Name, 
            LicensePlate = equipment.StateNumber, 
            Code = equipment.Code,
            ShortName = equipment.ShortName,
            EquipmentGroupId = equipment.EquipmentGroup!.Id
        };
        var client = clientFactory.CreateEquipmentClient();
        var result = await client.CreateEquipmentAsync(request, headers);
        return result.Id;
    }

    public async Task<bool> UpdateEquipmentAsync(RequestManagement.Common.Models.Equipment equipment)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var request = new UpdateEquipmentRequest
        {
            Id = equipment.Id, 
            Name = equipment.Name, 
            LicensePlate = equipment.StateNumber, 
            Code = equipment.Code,
            ShortName = equipment.ShortName,
            EquipmentGroupId = equipment.EquipmentGroupId ?? 1
        };
        var client = clientFactory.CreateEquipmentClient();
        var result = await client.UpdateEquipmentAsync(request, headers);
        return result.Success;
    }

    public async Task<bool> DeleteEquipmentAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var request = new DeleteEquipmentRequest { Id = id};
        var client = clientFactory.CreateEquipmentClient();
        var result = await client.DeleteEquipmentAsync(request, headers);
        return result.Success;
    }

    public async Task<List<RequestManagement.Common.Models.Equipment>> GetAllEquipmentAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateEquipmentClient();
        var response = await client.GetAllEquipmentAsync(new GetAllEquipmentRequest { Filter = filter }, headers);
        return response.Equipment.Select(equipment => new RequestManagement.Common.Models.Equipment
        {
            Id = equipment.Id, 
            Name = equipment.Name, 
            StateNumber = equipment.LicensePlate, 
            Code = equipment.Code,
            ShortName = equipment.ShortName,
            EquipmentGroup = new RequestManagement.Common.Models.EquipmentGroup { Id = equipment.EquipmentGroup.Id, Name = equipment.EquipmentGroup.Name }
        }).ToList();
    }
}