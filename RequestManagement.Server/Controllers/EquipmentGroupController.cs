using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class EquipmentGroupController(IEquipmentGroupService equipmentGroupService, ILogger<EquipmentGroupController> logger)
    : EquipmentGroupService.EquipmentGroupServiceBase
{
    private readonly IEquipmentGroupService _equipmentGroupService = equipmentGroupService ?? throw new ArgumentNullException(nameof(equipmentGroupService));
    private readonly ILogger<EquipmentGroupController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllEquipmentGroupsResponse> GetAllEquipmentGroups(GetAllEquipmentGroupsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all EquipmentGroup by filter");

        var equipmentGroupList = await _equipmentGroupService.GetAllEquipmentGroupsAsync(request.Filter);
        var response = new GetAllEquipmentGroupsResponse();
        response.EquipmentGroups.AddRange(equipmentGroupList.Select(e => new EquipmentGroup
        {
            Id = e.Id,
            Name = e.Name,
            Equipments =
            {
                e.Equipments.Select(equipment => new EquipmentGroupEquipment
                {
                    Id = equipment.Id, 
                    Name = equipment.Name,
                    ShortName = equipment.ShortName,
                    Code = equipment.Code,
                    LicensePlate = equipment.StateNumber
                }).ToList()
            },
        }));

        return response;
    }
    public override async Task<CreateEquipmentGroupResponse> CreateEquipmentGroup(CreateEquipmentGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new equipmentGroup with full name: {Name}", request.EquipmentGroup.Name);

        var equipmentGroup = new Common.Models.EquipmentGroup
        {
            Name = request.EquipmentGroup.Name,
        };

        var id = await _equipmentGroupService.CreateEquipmentGroupAsync(equipmentGroup);
        return new CreateEquipmentGroupResponse { Id = id };
    }
    public override async Task<UpdateEquipmentGroupResponse> UpdateEquipmentGroup(UpdateEquipmentGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating equipmentGroup with ID: {Id}", request.EquipmentGroup.Id);

        var equipmentGroup = new Common.Models.EquipmentGroup
        {
            Id = request.EquipmentGroup.Id,
            Name = request.EquipmentGroup.Name,
        };

        var success = await _equipmentGroupService.UpdateEquipmentGroupAsync(equipmentGroup);
        return new UpdateEquipmentGroupResponse { Success = success };
    }
    public override async Task<DeleteEquipmentGroupResponse> DeleteEquipmentGroup(DeleteEquipmentGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting equipmentGroup with ID: {Id}", request.Id);

        var success = await _equipmentGroupService.DeleteEquipmentGroupAsync(request.Id);
        return new DeleteEquipmentGroupResponse { Success = success };
    }
}