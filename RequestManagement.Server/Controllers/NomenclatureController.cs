using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class NomenclatureController(INomenclatureService nomenclatureService, ILogger<NomenclatureController> logger)
    : NomenclatureService.NomenclatureServiceBase
{
    private readonly INomenclatureService _nomenclatureService = nomenclatureService ?? throw new ArgumentNullException(nameof(nomenclatureService));
    private readonly ILogger<NomenclatureController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public override async Task<GetAllNomenclaturesResponse> GetAllNomenclatures(GetAllNomenclaturesRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all nomenclatures by filter");

        var nomenclatureList = await _nomenclatureService.GetAllNomenclaturesAsync(request.Filter);
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
        _logger.LogInformation("Creating new nomenclature with name: {Name}", request.Nomenclature.Name);

        var nomenclature = new RequestManagement.Common.Models.Nomenclature
        {
            Name = request.Nomenclature.Name,
            Code = request.Nomenclature.Code,
            UnitOfMeasure = request.Nomenclature.UnitOfMeasure,
            Article = request.Nomenclature.Article
        };

        var id = await _nomenclatureService.CreateNomenclatureAsync(nomenclature);
        return new CreateNomenclatureResponse { Id = id };
    }

    public override async Task<UpdateNomenclatureResponse> UpdateNomenclature(UpdateNomenclatureRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating nomenclature with ID: {Id}", request.Nomenclature.Id);

        var nomenclature = new RequestManagement.Common.Models.Nomenclature
        {
            Id = request.Nomenclature.Id,
            Name = request.Nomenclature.Name,
            Code = request.Nomenclature.Code,
            UnitOfMeasure = request.Nomenclature.UnitOfMeasure,
            Article = request.Nomenclature.Article
        };

        var success = await _nomenclatureService.UpdateNomenclatureAsync(nomenclature);
        return new UpdateNomenclatureResponse { Success = success };
    }

    public override async Task<DeleteNomenclatureResponse> DeleteNomenclature(DeleteNomenclatureRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting nomenclature with ID: {Id}", request.Id);

        var success = await _nomenclatureService.DeleteNomenclatureAsync(request.Id);
        return new DeleteNomenclatureResponse { Success = success };
    }
}