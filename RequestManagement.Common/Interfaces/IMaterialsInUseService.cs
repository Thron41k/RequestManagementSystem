using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IMaterialsInUseService
{
    Task<List<MaterialsInUse>> GetAllMaterialsInUseAsync(int financiallyResponsiblePersonId, string filter = "");
    Task<int> CreateMaterialsInUseAsync(MaterialsInUse materialsInUse);
    Task<bool> UploadMaterialsInUseAsync(List<MaterialsInUseForUpload> materialsInUse);
    Task<bool> UpdateMaterialsInUseAsync(MaterialsInUse materialsInUse);
    Task<bool> DeleteMaterialsInUseAsync(int id);
}