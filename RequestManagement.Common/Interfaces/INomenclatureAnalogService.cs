using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models;

namespace RequestManagement.Common.Interfaces
{
    public interface INomenclatureAnalogService
    {
        Task<List<Nomenclature>> GetAllNomenclatureAnalogsAsync(int filter);
        Task<int> AddNomenclatureAnalogAsync(NomenclatureAnalog nomenclatureAnalog);
        Task<bool> DeleteNomenclatureAnalogAsync(int originalId, int analogId);
    }
}
