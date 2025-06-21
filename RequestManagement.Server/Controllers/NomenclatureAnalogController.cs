using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

public class NomenclatureAnalogController(INomenclatureAnalogService nomenclatureAnalogService, ILogger<RequestController> logger)
    : NomenclatureAnalogService.NomenclatureAnalogServiceBase
{
    public override async Task<GetAllNomenclatureAnalogsResponse> GetAllNomenclatureAnalogs(GetAllNomenclatureAnalogsRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        logger.LogInformation("Getting all nomenclatureAnalog by filter");

        var nomenclatureAnalogList = await nomenclatureAnalogService.GetAllNomenclatureAnalogsAsync(request.Id);
        var response = new GetAllNomenclatureAnalogsResponse();
        response.Nomenclature.AddRange(nomenclatureAnalogList.Select(e => new AnalogNomenclature
        {
            Id = e.Id,
            Name = e.Name,
            Article = e.Article,
            Code = e.Code,
            UnitOfMeasure = e.UnitOfMeasure
        }));
        return response;
    }
    public override async Task<AddNomenclatureAnalogResponse> AddNomenclatureAnalog(AddNomenclatureAnalogRequest request, ServerCallContext context)
    {
        logger.LogInformation("Creating new nomenclatureAnalog");

        var nomenclatureAnalog = new Common.Models.NomenclatureAnalog
        {
            AnalogId = request.Analog,
            OriginalId = request.Original
        };

        var id = await nomenclatureAnalogService.AddNomenclatureAnalogAsync(nomenclatureAnalog);
        return new AddNomenclatureAnalogResponse { Id = id };
    }
    public override async Task<DeleteNomenclatureAnalogResponse> DeleteNomenclatureAnalog(DeleteNomenclatureAnalogRequest request, ServerCallContext context)
    {
        logger.LogInformation("Deleting nomenclatureAnalog with ID: {Id}", request.Analog);

        var success = await nomenclatureAnalogService.DeleteNomenclatureAnalogAsync(request.Original, request.Analog);
        return new DeleteNomenclatureAnalogResponse { Success = success };
    }
}