using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers;

/// <summary>
/// gRPC-контроллер для работы с оборудованием
/// </summary>
public class EquipmentController(IEquipmentService equipmentService, ILogger<EquipmentController> logger) : EquipmentService.EquipmentServiceBase
{
    private readonly IEquipmentService _requestService = equipmentService ?? throw new ArgumentNullException(nameof(equipmentService));
    private readonly ILogger<EquipmentController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Получает список всех единиц оборудования
    /// </summary>
    public override async Task<GetAllEquipmentResponse> GetAllEquipment(GetAllEquipmentRequest request, ServerCallContext context)
    {
        var user = context.GetHttpContext().User;
        if (user.Identity is { IsAuthenticated: false })
        {
            throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
        }

        _logger.LogInformation("Getting all equipment");

        var equipmentList = await _requestService.GetAllEquipmentAsync(request.Filter);
        var response = new GetAllEquipmentResponse();
        response.Equipment.AddRange(equipmentList.Select(e => new Equipment
        {
            Id = e.Id,
            Name = e.Name,
            LicensePlate = e.StateNumber ?? "",
            Code = e.Code,
            ShortName = e.ShortName ?? ""
        }));

        return response;
    }

    /// <summary>
    /// Создает новую единицу оборудования
    /// </summary>
    public override async Task<CreateEquipmentResponse> CreateEquipment(CreateEquipmentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating new equipment with name: {Name}", request.Name);

        var equipment = new RequestManagement.Common.Models.Equipment
        {
            Name = request.Name,
            StateNumber = request.LicensePlate,
            Code = request.Code,
            ShortName = request.ShortName
        };

        var id = await _requestService.CreateEquipmentAsync(equipment);
        return new CreateEquipmentResponse { Id = id };
    }

    /// <summary>
    /// Обновляет существующую единицу оборудования
    /// </summary>
    public override async Task<UpdateEquipmentResponse> UpdateEquipment(UpdateEquipmentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Updating equipment with ID: {Id}", request.Id);

        var equipment = new RequestManagement.Common.Models.Equipment
        {
            Id = request.Id,
            Name = request.Name,
            StateNumber = request.LicensePlate,
            Code = request.Code,
            ShortName = request.ShortName
        };

        var success = await _requestService.UpdateEquipmentAsync(equipment);
        return new UpdateEquipmentResponse { Success = success };
    }

    /// <summary>
    /// Удаляет единицу оборудования
    /// </summary>
    public override async Task<DeleteEquipmentResponse> DeleteEquipment(DeleteEquipmentRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Deleting equipment with ID: {Id}", request.Id);

        var success = await _requestService.DeleteEquipmentAsync(request.Id);
        return new DeleteEquipmentResponse { Success = success };
    }
}