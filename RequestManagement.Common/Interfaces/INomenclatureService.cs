using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces;

public interface INomenclatureService
{
    Task<List<Nomenclature>> GetAllNomenclaturesAsync(string filter = "");
    Task<int> CreateNomenclatureAsync(Nomenclature nomenclature);
    Task<bool> UpdateNomenclatureAsync(Nomenclature nomenclature);
    Task<bool> DeleteNomenclatureAsync(int id);
}