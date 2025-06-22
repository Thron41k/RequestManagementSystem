using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface IDefectGroupService
{
    Task<List<DefectGroup>> GetAllDefectGroupsAsync(string filter = "");
    Task<int> CreateDefectGroupAsync(DefectGroup defectGroup);
    Task<bool> UpdateDefectGroupAsync(DefectGroup defectGroup);
    Task<bool> DeleteDefectGroupAsync(int id);
}