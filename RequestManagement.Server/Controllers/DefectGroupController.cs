using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class DefectGroupController(IDefectGroupService defectGroupService, ILogger<DefectGroupController> logger)
    : DefectGroupService.DefectGroupServiceBase
{
    private readonly IDefectGroupService _defectGroupService = defectGroupService ?? throw new ArgumentNullException(nameof(defectGroupService));
    private readonly ILogger<DefectGroupController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllDefectGroupsResponse> GetAllDefectGroups(GetAllDefectGroupsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all drivers by filter");

        var defectGroupList = await _defectGroupService.GetAllDefectGroupsAsync(request.Filter);
        var response = new GetAllDefectGroupsResponse();
        response.DefectGroup.AddRange(defectGroupList.Select(e => new DefectGroup
        {
            Id = e.Id,
            Name = e.Name,
        }));

        return response;
    }
    public override async Task<CreateDefectGroupResponse> CreateDefectGroup(CreateDefectGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new defectGroup with full name: {Name}", request.DefectGroup.Name);

        var defectGroup = new Common.Models.DefectGroup
        {
            Name = request.DefectGroup.Name,
        };

        var id = await _defectGroupService.CreateDefectGroupAsync(defectGroup);
        return new CreateDefectGroupResponse { Id = id };
    }
    public override async Task<UpdateDefectGroupResponse> UpdateDefectGroup(UpdateDefectGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating defectGroup with ID: {Id}", request.DefectGroup.Id);

        var defectGroup = new Common.Models.DefectGroup
        {
            Id = request.DefectGroup.Id,
            Name = request.DefectGroup.Name,
        };

        var success = await _defectGroupService.UpdateDefectGroupAsync(defectGroup);
        return new UpdateDefectGroupResponse { Success = success };
    }
    public override async Task<DeleteDefectGroupResponse> DeleteDefectGroup(DeleteDefectGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting defectGroup with ID: {Id}", request.Id);

        var success = await _defectGroupService.DeleteDefectGroupAsync(request.Id);
        return new DeleteDefectGroupResponse { Success = success };
    }
}