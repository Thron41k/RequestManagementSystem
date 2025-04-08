using Grpc.Core;
using RequestManagement.Common.Interfaces;
using RequestManagement.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace RequestManagement.Server.Controllers
{
    /// <summary>
    /// gRPC-контроллер для работы с оборудованием
    /// </summary>
    public class RequestController : RequestService.RequestServiceBase
    {
        private readonly IRequestService _requestService;
        private readonly ILogger<RequestController> _logger;

        public RequestController(IRequestService requestService, ILogger<RequestController> logger)
        {
            _requestService = requestService ?? throw new ArgumentNullException(nameof(requestService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Получает список всех единиц оборудования
        /// </summary>
        public override async Task<GetAllEquipmentResponse> GetAllEquipment(GetAllEquipmentRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (!user.Identity.IsAuthenticated)
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
                LicensePlate = e.StateNumber ?? ""
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
                StateNumber = request.LicensePlate
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
                StateNumber = request.LicensePlate
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

        public override async Task<GetAllDriversResponse> GetAllDrivers(GetAllDriversRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            _logger.LogInformation("Getting all drivers by filter");

            var equipmentList = await _requestService.GetAllDriversAsync(request.Filter);
            var response = new GetAllDriversResponse();
            response.Drivers.AddRange(equipmentList.Select(e => new Driver
            {
                Id = e.Id,
                FullName = e.FullName,
                ShortName = e.ShortName,
                Position = e.Position
            }));

            return response;
        }

        public override async Task<CreateDriverResponse> CreateDriver(CreateDriverRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Creating new driver with full name and position: {Name} - {Position}", request.Driver.FullName,request.Driver.Position);

            var driver = new RequestManagement.Common.Models.Driver
            {
                FullName = request.Driver.FullName,
                ShortName = request.Driver.ShortName,
                Position = request.Driver.Position
            };

            var id = await _requestService.CreateDriverAsync(driver);
            return new CreateDriverResponse { Id = id };
        }

        public override async Task<UpdateDriverResponse> UpdateDriver(UpdateDriverRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Updating driver with ID: {Id}", request.Driver.Id);

            var driver = new RequestManagement.Common.Models.Driver
            {
                Id = request.Driver.Id,
                FullName = request.Driver.FullName,
                ShortName = request.Driver.ShortName,
                Position = request.Driver.Position
            };

            var success = await _requestService.UpdateDriverAsync(driver);
            return new UpdateDriverResponse { Success = success };
        }

        public override async Task<DeleteDriverResponse> DeleteDriver(DeleteDriverRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Deleting driver with ID: {Id}", request.Id);

            var success = await _requestService.DeleteDriverAsync(request.Id);
            return new DeleteDriverResponse { Success = success };
        }

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
    }
}