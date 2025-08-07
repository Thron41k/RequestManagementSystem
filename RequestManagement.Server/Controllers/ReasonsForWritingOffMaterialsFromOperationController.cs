using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class ReasonsForWritingOffMaterialsFromOperationController(IReasonsForWritingOffMaterialsFromOperationService reasonsForWritingOffMaterialsFromOperationService, ILogger<ReasonsForWritingOffMaterialsFromOperationController> logger)
    : ReasonsForWritingOffMaterialsFromOperationService.ReasonsForWritingOffMaterialsFromOperationServiceBase
{
    private readonly IReasonsForWritingOffMaterialsFromOperationService _reasonsForWritingOffMaterialsFromOperationService = reasonsForWritingOffMaterialsFromOperationService ?? throw new ArgumentNullException(nameof(reasonsForWritingOffMaterialsFromOperationService));
    private readonly ILogger<ReasonsForWritingOffMaterialsFromOperationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllReasonsForWritingOffMaterialsFromOperationResponse> GetAllReasonsForWritingOffMaterialsFromOperation(GetAllReasonsForWritingOffMaterialsFromOperationRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all reasonsForWritingOffMaterialsFromOperations by filter");

        var reasonsForWritingOffMaterialsFromOperationList = await _reasonsForWritingOffMaterialsFromOperationService.GetAllReasonsForWritingOffMaterialsFromOperationAsync(request.Filter);
        var response = new GetAllReasonsForWritingOffMaterialsFromOperationResponse();
        response.ReasonsForWritingOffMaterialsFromOperation.AddRange(reasonsForWritingOffMaterialsFromOperationList.Select(e => new ReasonsForWritingOffMaterialsFromOperation
        {
            Id = e.Id,
            Reason = e.Reason
        }));

        return response;
    }

    public override async Task<CreateReasonsForWritingOffMaterialsFromOperationResponse> CreateReasonsForWritingOffMaterialsFromOperation(CreateReasonsForWritingOffMaterialsFromOperationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new reasonsForWritingOffMaterialsFromOperation with Reason: {Reason}", request.ReasonsForWritingOffMaterialsFromOperation.Reason);

        var reasonsForWritingOffMaterialsFromOperation = new Common.Models.ReasonsForWritingOffMaterialsFromOperation
        {
            Reason = request.ReasonsForWritingOffMaterialsFromOperation.Reason
        };

        var id = await _reasonsForWritingOffMaterialsFromOperationService.CreateReasonsForWritingOffMaterialsFromOperationAsync(reasonsForWritingOffMaterialsFromOperation);
        return new CreateReasonsForWritingOffMaterialsFromOperationResponse { Id = id };
    }
    public override async Task<UpdateReasonsForWritingOffMaterialsFromOperationResponse> UpdateReasonsForWritingOffMaterialsFromOperation(UpdateReasonsForWritingOffMaterialsFromOperationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating reasonsForWritingOffMaterialsFromOperation with ID: {Id}", request.ReasonsForWritingOffMaterialsFromOperation.Id);

        var reasonsForWritingOffMaterialsFromOperation = new Common.Models.ReasonsForWritingOffMaterialsFromOperation
        {
            Id = request.ReasonsForWritingOffMaterialsFromOperation.Id,
            Reason = request.ReasonsForWritingOffMaterialsFromOperation.Reason
        };

        var success = await _reasonsForWritingOffMaterialsFromOperationService.UpdateReasonsForWritingOffMaterialsFromOperationAsync(reasonsForWritingOffMaterialsFromOperation);
        return new UpdateReasonsForWritingOffMaterialsFromOperationResponse { Success = success };
    }

    public override async Task<DeleteReasonsForWritingOffMaterialsFromOperationResponse> DeleteReasonsForWritingOffMaterialsFromOperation(DeleteReasonsForWritingOffMaterialsFromOperationRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting reasonsForWritingOffMaterialsFromOperation with ID: {Id}", request.Id);

        var success = await _reasonsForWritingOffMaterialsFromOperationService.DeleteReasonsForWritingOffMaterialsFromOperationAsync(request.Id);
        return new DeleteReasonsForWritingOffMaterialsFromOperationResponse { Success = success };
    }
}