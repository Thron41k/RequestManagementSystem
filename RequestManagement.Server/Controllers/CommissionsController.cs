using Grpc.Core;
using RequestManagement.Common.Interfaces;

namespace RequestManagement.Server.Controllers
{
    public class CommissionsController(ICommissionsService commissionsService, ILogger<RequestController> logger)
        : CommissionsService.CommissionsServiceBase
    {
        public override async Task<GetAllCommissionsResponse> GetAllCommissions(GetAllCommissionsRequest request, ServerCallContext context)
        {
            var user = context.GetHttpContext().User;
            if (user.Identity is { IsAuthenticated: false })
            {
                throw new RpcException(new Status(StatusCode.Unauthenticated, "User is not authenticated"));
            }

            logger.LogInformation("Getting all commissions by filter");

            var commissionsList = await commissionsService.GetAllCommissionsAsync(request.Filter);
            var response = new GetAllCommissionsResponse();
            response.Commissions.AddRange(commissionsList.Select(e => new Commissions
            {
                Id = e.Id,
                Name = e.Name,
                ApproveAct = e.ApproveForAct != null ? new CommissionsDriver
                {
                    Id = e.ApproveForAct.Id,
                    FullName = e.ApproveForAct.FullName,
                    ShortName = e.ApproveForAct.ShortName,
                    Position = e.ApproveForAct.Position
                } : null,
                ApproveDefectAndLimit = e.ApproveForDefectAndLimit != null ? new CommissionsDriver
                {
                    Id = e.ApproveForDefectAndLimit.Id,
                    FullName = e.ApproveForDefectAndLimit.FullName,
                    ShortName = e.ApproveForDefectAndLimit.ShortName,
                    Position = e.ApproveForDefectAndLimit.Position
                } : null,
                Chairman = e.Chairman != null ? new CommissionsDriver
                {
                    Id = e.Chairman.Id,
                    FullName = e.Chairman.FullName,
                    ShortName = e.Chairman.ShortName,
                    Position = e.Chairman.Position
                } : null,
                Member1 = e.Member1 != null ? new CommissionsDriver
                {
                    Id = e.Member1.Id,
                    FullName = e.Member1.FullName,
                    ShortName = e.Member1.ShortName,
                    Position = e.Member1.Position
                } : null,
                Member2 = e.Member2 != null ? new CommissionsDriver
                {
                    Id = e.Member2.Id,
                    FullName = e.Member2.FullName,
                    ShortName = e.Member2.ShortName,
                    Position = e.Member2.Position
                } : null,
                Member3 = e.Member3 != null ? new CommissionsDriver
                {
                    Id = e.Member3.Id,
                    FullName = e.Member3.FullName,
                    ShortName = e.Member3.ShortName,
                    Position = e.Member3.Position
                } : null,
                Member4 = e.Member4 != null ? new CommissionsDriver
                {
                    Id = e.Member4.Id,
                    FullName = e.Member4.FullName,
                    ShortName = e.Member4.ShortName,
                    Position = e.Member4.Position
                } : null
            }));
            return response;
        }

        public override async Task<CreateCommissionsResponse> CreateCommissions(CreateCommissionsRequest request, ServerCallContext context)
        {
            logger.LogInformation("Creating new commissions with name: {Name}", request.Name);

            var commissions = new RequestManagement.Common.Models.Commissions
            {
                Name = request.Name,
                ApproveForActId = request.ApproveActId == 0 ? 1 : request.ApproveActId,
                ApproveForDefectAndLimitId = request.ApproveDefectAndLimitId == 0 ? 1 : request.ApproveDefectAndLimitId,
                ChairmanId = request.ChairmanId == 0 ? 1 : request.ChairmanId,
                Member1Id = request.Member1Id == 0 ? 1 : request.Member1Id,
                Member2Id = request.Member2Id == 0 ? 1 : request.Member2Id,
                Member3Id = request.Member3Id == 0 ? 1 : request.Member3Id,
                Member4Id = request.Member4Id == 0 ? 1 : request.Member4Id
            };

            var id = await commissionsService.CreateCommissionsAsync(commissions);
            return new CreateCommissionsResponse { Id = id };
        }

        public override async Task<UpdateCommissionsResponse> UpdateCommissions(UpdateCommissionsRequest request, ServerCallContext context)
        {
            try
            {
                logger.LogInformation("Updating commissions with ID: {Id}", request.Id);

                var commissions = new RequestManagement.Common.Models.Commissions
                {
                    Id = request.Id,
                    Name = request.Name,
                    ApproveForActId = request.ApproveActId == 0 ? 1 : request.ApproveActId,
                    ApproveForDefectAndLimitId = request.ApproveDefectAndLimitId == 0 ? 1 : request.ApproveDefectAndLimitId,
                    ChairmanId = request.ChairmanId == 0 ? 1 : request.ChairmanId,
                    Member1Id = request.Member1Id == 0 ? 1 : request.Member1Id,
                    Member2Id = request.Member2Id == 0 ? 1 : request.Member2Id,
                    Member3Id = request.Member3Id == 0 ? 1 : request.Member3Id,
                    Member4Id = request.Member4Id == 0 ? 1 : request.Member4Id
                };

                var success = await commissionsService.UpdateCommissionsAsync(commissions);
                return new UpdateCommissionsResponse { Success = success };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating commissions with ID: {Id}", request.Id);
                return new UpdateCommissionsResponse { Success = false};
            }
        }

        public override async Task<DeleteCommissionsResponse> DeleteCommissions(DeleteCommissionsRequest request, ServerCallContext context)
        {
            logger.LogInformation("Deleting commissions with ID: {Id}", request.Id);

            var success = await commissionsService.DeleteCommissionsAsync(request.Id);
            return new DeleteCommissionsResponse { Success = success };
        }
    }
}
