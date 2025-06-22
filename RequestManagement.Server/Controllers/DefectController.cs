using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class DefectController(IDefectService defectService, ILogger<DefectController> logger)
    : DefectService.DefectServiceBase
{
    private readonly IDefectService _defectService = defectService ?? throw new ArgumentNullException(nameof(defectService));
    private readonly ILogger<DefectController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllDefectsResponse> GetAllDefects(GetAllDefectsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all defects by filter");

        var defectList = await _defectService.GetAllDefectsAsync(request.Filter);
        var response = new GetAllDefectsResponse();
        response.Defect.AddRange(defectList.Select(e => new Defect
        {
            Id = e.Id,
            Name = e.Name,
            DefectGroupId = e.DefectGroupId
        }));

        return response;
    }

    public override async Task<CreateDefectResponse> CreateDefect(CreateDefectRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new defect with name: {Name}", request.Defect.Name);

        var defect = new Common.Models.Defect
        {
            Name = request.Defect.Name,
            DefectGroupId = request.Defect.DefectGroupId
        };

        var id = await _defectService.CreateDefectAsync(defect);
        return new CreateDefectResponse { Id = id };
    }
    public override async Task<UpdateDefectResponse> UpdateDefect(UpdateDefectRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating defect with ID: {Id}", request.Defect.Id);

        var defect = new Common.Models.Defect
        {
            Id = request.Defect.Id,
            Name = request.Defect.Name,
            DefectGroupId = request.Defect.DefectGroupId
        };

        var success = await _defectService.UpdateDefectAsync(defect);
        return new UpdateDefectResponse { Success = success };
    }

    public override async Task<DeleteDefectResponse> DeleteDefect(DeleteDefectRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting defect with ID: {Id}", request.Id);

        var success = await _defectService.DeleteDefectAsync(request.Id);
        return new DeleteDefectResponse { Success = success };
    }
}