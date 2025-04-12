using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IDefectService
{
    Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "");
    Task<int> CreateDefectGroupAsync(DefectGroup defectGroup);
    Task<bool> UpdateDefectGroupAsync(DefectGroup defectGroup);
    Task<bool> DeleteDefectGroupAsync(int id);

    Task<List<Defect>> GetAllDefectsAsync(string filter = "");
    Task<int> CreateDefectAsync(Defect defect);
    Task<bool> UpdateDefectAsync(Defect defect);
    Task<bool> DeleteDefectAsync(int id);
}