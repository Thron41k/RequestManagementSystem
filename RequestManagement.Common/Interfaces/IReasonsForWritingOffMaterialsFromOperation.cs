using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IReasonsForWritingOffMaterialsFromOperationService
{
    Task<List<ReasonsForWritingOffMaterialsFromOperation>> GetAllReasonsForWritingOffMaterialsFromOperationAsync(string filter = "");
    Task<int> CreateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation);
    Task<bool> UpdateReasonsForWritingOffMaterialsFromOperationAsync(ReasonsForWritingOffMaterialsFromOperation reasonsForWritingOffMaterialsFromOperation);
    Task<bool> DeleteReasonsForWritingOffMaterialsFromOperationAsync(int id);
}