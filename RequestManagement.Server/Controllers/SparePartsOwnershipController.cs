using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using RequestManagement.Server.Converters;

namespace RequestManagement.Server.Controllers;

/// <summary>
/// gRPC-контроллер для работы с оборудованием
/// </summary>
public class SparePartsOwnershipController(ISparePartsOwnershipService sparePartsOwnershipService, ILogger<SparePartsOwnershipController> logger)
    : SparePartsOwnershipService.SparePartsOwnershipServiceBase
{
    private readonly ISparePartsOwnershipService _requestService = sparePartsOwnershipService ?? throw new ArgumentNullException(nameof(sparePartsOwnershipService));
    private readonly ILogger<SparePartsOwnershipController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllSparePartsOwnershipsResponse> GetAllSparePartsOwnerships(GetAllSparePartsOwnershipsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all sparePartsOwnerships by filter");

        var sparePartsOwnershipList = await _requestService.GetAllSparePartsOwnershipsAsync(request.EqipmentGroupId,request.WarehouseId);
        var response = SparePartsOwnershipConverter.MapToGrpcResponse(sparePartsOwnershipList);
        return response;
    }

    public override async Task<CreateSparePartsOwnershipResponse> CreateSparePartsOwnership(CreateSparePartsOwnershipRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new sparePartsOwnership with Comment: {Name}", request.Comment);

        var sparePartsOwnership = new RequestManagement.Common.Models.SparePartsOwnership
        {
            RequiredQuantity = request.RequiredQuantity,
            Comment = request.Comment,
            EquipmentGroupId = request.EqipmentGroupId,
            NomenclatureId = request.NomenclatureId
        };

        var id = await _requestService.CreateSparePartsOwnershipAsync(sparePartsOwnership);
        return new CreateSparePartsOwnershipResponse { SparePartsOwnershipId = id };
    }

    public override async Task<UpdateSparePartsOwnershipResponse> UpdateSparePartsOwnership(UpdateSparePartsOwnershipRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating sparePartsOwnership with ID: {Id}", request.SparePartsOwnershipId);

        var sparePartsOwnership = new RequestManagement.Common.Models.SparePartsOwnership
        {
            Id = request.SparePartsOwnershipId,
            RequiredQuantity = request.RequiredQuantity,
            Comment = request.Comment,
            EquipmentGroupId = request.EqipmentGroupId,
            NomenclatureId = request.NomenclatureId
        };

        var success = await _requestService.UpdateSparePartsOwnershipAsync(sparePartsOwnership);
        return new UpdateSparePartsOwnershipResponse { Success = success };
    }

    public override async Task<DeleteSparePartsOwnershipResponse> DeleteSparePartsOwnership(DeleteSparePartsOwnershipRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting sparePartsOwnership with ID: {Id}", request.Id);

        var success = await _requestService.DeleteSparePartsOwnershipAsync(request.Id);
        return new DeleteSparePartsOwnershipResponse { Success = success };
    }
}