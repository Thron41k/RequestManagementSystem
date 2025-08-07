using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Server.Converters;

namespace RequestManagement.Server.Controllers;

public class MaterialsInUseController(IMaterialsInUseService materialsInUseService, ILogger<MaterialsInUseController> logger)
    : MaterialsInUseService.MaterialsInUseServiceBase
{
    private readonly IMaterialsInUseService _materialsInUseService = materialsInUseService ?? throw new ArgumentNullException(nameof(materialsInUseService));
    private readonly ILogger<MaterialsInUseController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllMaterialsInUseResponse> GetAllMaterialsInUse(GetAllMaterialsInUseRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }
        _logger.LogInformation("Getting all materialsInUses by filter");
        var materialsInUseList = await _materialsInUseService.GetAllMaterialsInUseAsync(request.FinanciallyResponsiblePersonId, request.Filter);
        return MaterialsInUseConverter.ToGrpc(materialsInUseList);
    }

    public override async Task<UploadMaterialsInUseResponse> UploadMaterialsInUse(UploadMaterialsInUseRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }
        _logger.LogInformation("Upload materialsInUses from Client");
        var result = await _materialsInUseService.UploadMaterialsInUseAsync(request.MaterialsInUse.Select(x=> new Common.Models.MaterialsInUseForUpload
        {
            DocumentNumber = x.DocumentNumber,
            Date = x.Date,
            Quantity = (decimal)x.Quantity,
            NomenclatureCode = x.NomenclatureCode,
            NomenclatureName = x.NomenclatureName,
            NomenclatureArticle = x.NomenclatureArticle,
            NomenclatureUnitOfMeasure = x.NomenclatureUnitOfMeasure,
            EquipmentName = x.EquipmentName,
            EquipmentCode = x.EquipmentCode,
            FinanciallyResponsiblePersonFullName = x.FinanciallyResponsiblePersonFullName,
            FinanciallyResponsiblePersonCode = x.FinanciallyResponsiblePersonFullCode
        }).ToList());
        return new UploadMaterialsInUseResponse{Success = result };
    }

    public override async Task<CreateMaterialsInUseResponse> CreateMaterialsInUse(CreateMaterialsInUseRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new MaterialsInUse with DocumentNumber: {Name}", request.MaterialsInUse.DocumentNumber);

        var materialsInUse = new Common.Models.MaterialsInUse
        {
            DocumentNumber = request.MaterialsInUse.DocumentNumber,
            Date = DateTime.Parse(request.MaterialsInUse.Date),
            Quantity = (decimal)request.MaterialsInUse.Quantity,
            NomenclatureId = request.MaterialsInUse.NomenclatureId,
            EquipmentId = request.MaterialsInUse.EquipmentId,
            FinanciallyResponsiblePersonId = request.MaterialsInUse.FinanciallyResponsiblePersonId,
            IsOut = request.MaterialsInUse.IsOut,
            ReasonForWriteOff = new Common.Models.ReasonsForWritingOffMaterialsFromOperation
            {
                Id = request.MaterialsInUse.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Id,
                Reason = request.MaterialsInUse.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Reason
            },
            DocumentNumberForWriteOff = request.MaterialsInUse.DocumentNumberForWriteOff,
            DateForWriteOff = DateTime.Parse(request.MaterialsInUse.DateForWriteOff)
        };

        var id = await _materialsInUseService.CreateMaterialsInUseAsync(materialsInUse);
        return new CreateMaterialsInUseResponse { Id = id };
    }
    public override async Task<UpdateMaterialsInUseResponse> UpdateMaterialsInUse(UpdateMaterialsInUseRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating materialsInUse with ID: {Id}", request.MaterialsInUse.Id);
        var materialsInUse = new Common.Models.MaterialsInUse
        {
            Id = request.MaterialsInUse.Id,
            DocumentNumber = request.MaterialsInUse.DocumentNumber,
            Date = DateTime.Parse(request.MaterialsInUse.Date),
            Quantity = (decimal)request.MaterialsInUse.Quantity,
            NomenclatureId = request.MaterialsInUse.NomenclatureId,
            EquipmentId = request.MaterialsInUse.EquipmentId,
            FinanciallyResponsiblePersonId = request.MaterialsInUse.FinanciallyResponsiblePersonId,
            IsOut = request.MaterialsInUse.IsOut,
            ReasonForWriteOffId = request.MaterialsInUse.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Id,
            ReasonForWriteOff = new Common.Models.ReasonsForWritingOffMaterialsFromOperation
            {
                Id = request.MaterialsInUse.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Id,
                Reason = request.MaterialsInUse.MaterialsInUseDriverReasonsForWritingOffMaterialsFromOperation.Reason
            },
            DocumentNumberForWriteOff = request.MaterialsInUse.DocumentNumberForWriteOff,
            DateForWriteOff = DateTime.Parse(request.MaterialsInUse.DateForWriteOff)
        };
        var success = await _materialsInUseService.UpdateMaterialsInUseAsync(materialsInUse);
        return new UpdateMaterialsInUseResponse { Success = success };
    }

    public override async Task<DeleteMaterialsInUseResponse> DeleteMaterialsInUse(DeleteMaterialsInUseRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting materialsInUse with ID: {Id}", request.Id);
        var success = await _materialsInUseService.DeleteMaterialsInUseAsync(request.Id);
        return new DeleteMaterialsInUseResponse { Success = success };
    }
}