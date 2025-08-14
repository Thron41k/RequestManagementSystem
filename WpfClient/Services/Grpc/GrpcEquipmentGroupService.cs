using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using EquipmentGroup = RequestManagement.Common.Models.EquipmentGroup;

namespace RequestManagement.WpfClient.Services.Grpc;

public class GrpcEquipmentGroupService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : IEquipmentGroupService
{
    public async Task<List<EquipmentGroup>> GetAllEquipmentGroupsAsync(string filter = "")
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateEquipmentGroupClient();
        var response = await client.GetAllEquipmentGroupsAsync(new GetAllEquipmentGroupsRequest { Filter = filter }, headers);
        return response.EquipmentGroups.Select(equipmentGroup => new EquipmentGroup
        {
            Id = equipmentGroup.Id, 
            Name = equipmentGroup.Name,
            Equipments = equipmentGroup.Equipments.Select(equipment => new Common.Models.Equipment
            {
                Id = equipment.Id, 
                Name = equipment.Name,
                ShortName = equipment.ShortName,
                StateNumber = equipment.LicensePlate,
                Code = equipment.Code
            }).ToList()
        }).ToList();
    }

    public async Task<int> CreateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateEquipmentGroupClient();
        var result = await client.CreateEquipmentGroupAsync(new CreateEquipmentGroupRequest { EquipmentGroup = new Server.Controllers.EquipmentGroup { Name = equipmentGroup.Name } }, headers);
        return result.Id;
    }

    public async Task<bool> UpdateEquipmentGroupAsync(EquipmentGroup equipmentGroup)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateEquipmentGroupClient();
        var result = await client.UpdateEquipmentGroupAsync(new UpdateEquipmentGroupRequest { EquipmentGroup = new Server.Controllers.EquipmentGroup {Id = equipmentGroup.Id, Name = equipmentGroup.Name } }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteEquipmentGroupAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateEquipmentGroupClient();
        var result = await client.DeleteEquipmentGroupAsync(new DeleteEquipmentGroupRequest { Id = id }, headers);
        return result.Success;
    }
}