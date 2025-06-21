using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

/// <summary>
/// gRPC-контроллер для работы с оборудованием
/// </summary>
public class RequestController(IRequestService requestService, ILogger<RequestController> logger)
    : RequestService.RequestServiceBase
{
    private readonly IRequestService _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
    private readonly ILogger<RequestController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllDefectGroupsResponse> GetAllDefectGroups(GetAllDefectGroupsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all drivers by filter");

        var defectGroupList = await _requestService.GetAllDefectGroupsAsync(request.Filter);
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

        var id = await _requestService.CreateDefectGroupAsync(defectGroup);
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

        var success = await _requestService.UpdateDefectGroupAsync(defectGroup);
        return new UpdateDefectGroupResponse { Success = success };
    }
    public override async Task<DeleteDefectGroupResponse> DeleteDefectGroup(DeleteDefectGroupRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting defectGroup with ID: {Id}", request.Id);

        var success = await _requestService.DeleteDefectGroupAsync(request.Id);
        return new DeleteDefectGroupResponse { Success = success };
    }

    public override async Task<GetAllDefectsResponse> GetAllDefects(GetAllDefectsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all defects by filter");

        var defectList = await _requestService.GetAllDefectsAsync(request.Filter);
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

        var id = await _requestService.CreateDefectAsync(defect);
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

        var success = await _requestService.UpdateDefectAsync(defect);
        return new UpdateDefectResponse { Success = success };
    }

    public override async Task<DeleteDefectResponse> DeleteDefect(DeleteDefectRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting defect with ID: {Id}", request.Id);

        var success = await _requestService.DeleteDefectAsync(request.Id);
        return new DeleteDefectResponse { Success = success };
    }

    public override async Task<GetAllNomenclaturesResponse> GetAllNomenclatures(GetAllNomenclaturesRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        logger.LogInformation("Getting all nomenclatures by filter");

        var nomenclatureList = await requestService.GetAllNomenclaturesAsync(request.Filter);
        var response = new GetAllNomenclaturesResponse();
        response.Nomenclature.AddRange(nomenclatureList.Select(e => new Nomenclature
        {
            Id = e.Id,
            Name = e.Name,
            Code = e.Code,
            UnitOfMeasure = e.UnitOfMeasure,
            Article = e.Article
        }));

        return response;
    }
    public override async Task<CreateNomenclatureResponse> CreateNomenclature(CreateNomenclatureRequest request, ServerCallContext context)
    {
        logger.LogInformation("Creating new nomenclature with name: {Name}", request.Nomenclature.Name);

        var nomenclature = new RequestManagement.Common.Models.Nomenclature
        {
            Name = request.Nomenclature.Name,
            Code = request.Nomenclature.Code,
            UnitOfMeasure = request.Nomenclature.UnitOfMeasure,
            Article = request.Nomenclature.Article
        };

        var id = await requestService.CreateNomenclatureAsync(nomenclature);
        return new CreateNomenclatureResponse { Id = id };
    }

    public override async Task<UpdateNomenclatureResponse> UpdateNomenclature(UpdateNomenclatureRequest request, ServerCallContext context)
    {
        logger.LogInformation("Updating nomenclature with ID: {Id}", request.Nomenclature.Id);

        var nomenclature = new RequestManagement.Common.Models.Nomenclature
        {
            Id = request.Nomenclature.Id,
            Name = request.Nomenclature.Name,
            Code = request.Nomenclature.Code,
            UnitOfMeasure = request.Nomenclature.UnitOfMeasure,
            Article = request.Nomenclature.Article
        };

        var success = await requestService.UpdateNomenclatureAsync(nomenclature);
        return new UpdateNomenclatureResponse { Success = success };
    }

    public override async Task<DeleteNomenclatureResponse> DeleteNomenclature(DeleteNomenclatureRequest request, ServerCallContext context)
    {
        logger.LogInformation("Deleting nomenclature with ID: {Id}", request.Id);

        var success = await requestService.DeleteNomenclatureAsync(request.Id);
        return new DeleteNomenclatureResponse { Success = success };
    }
}