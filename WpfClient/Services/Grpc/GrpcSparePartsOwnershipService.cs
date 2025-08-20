using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Controllers;
using RequestManagement.WpfClient.Services.Interfaces;
using SparePartsOwnership = RequestManagement.Common.Models.SparePartsOwnership;
using SparePartsOwnershipConverter = RequestManagement.WpfClient.Converters.SparePartsOwnershipConverter;

namespace RequestManagement.WpfClient.Services.Grpc;

internal class GrpcSparePartsOwnershipService(IGrpcClientFactory clientFactory, AuthTokenStore tokenStore) : ISparePartsOwnershipService
{
    public async Task<List<SparePartsOwnership>> GetAllSparePartsOwnershipsAsync(int equipmentId, int warehouseId)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateSparePartsOwnershipClient();
        var response = await client.GetAllSparePartsOwnershipsAsync(new GetAllSparePartsOwnershipsRequest
        {
            EqipmentGroupId = equipmentId, 
            WarehouseId = warehouseId,
            
        }, headers);
        return SparePartsOwnershipConverter.MapFromGrpcResponse(response);
    }

    public async Task<int> CreateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateSparePartsOwnershipClient();
        var result = await client.CreateSparePartsOwnershipAsync(new CreateSparePartsOwnershipRequest
        {
            EqipmentGroupId = sparePartsOwnership.EquipmentGroupId,
            NomenclatureId = sparePartsOwnership.NomenclatureId,
            RequiredQuantity = sparePartsOwnership.RequiredQuantity,
            Comment = sparePartsOwnership.Comment
        }, headers);
        return result.SparePartsOwnershipId;
    }

    public async Task<bool> UpdateSparePartsOwnershipAsync(SparePartsOwnership sparePartsOwnership)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateSparePartsOwnershipClient();
        var result = await client.UpdateSparePartsOwnershipAsync(new UpdateSparePartsOwnershipRequest
        {
            SparePartsOwnershipId = sparePartsOwnership.Id,
            EqipmentGroupId = sparePartsOwnership.EquipmentGroupId,
            NomenclatureId = sparePartsOwnership.NomenclatureId,
            RequiredQuantity = sparePartsOwnership.RequiredQuantity,
            Comment = sparePartsOwnership.Comment ?? ""
        }, headers);
        return result.Success;
    }

    public async Task<bool> DeleteSparePartsOwnershipAsync(int id)
    {
        var headers = new Metadata();
        if (!string.IsNullOrEmpty(tokenStore.GetToken()))
        {
            headers.Add("Authorization", $"Bearer {tokenStore.GetToken()}");
        }
        var client = clientFactory.CreateSparePartsOwnershipClient();
        var result = await client.DeleteSparePartsOwnershipAsync(new DeleteSparePartsOwnershipRequest
        {
            Id = id
        }, headers);
        return result.Success;
    }
}