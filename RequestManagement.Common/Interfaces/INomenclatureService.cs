using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface INomenclatureService
    {
        Task<List<Nomenclature>> GetAllNomenclaturesAsync(string filter = "");
        Task<int> CreateNomenclatureAsync(Nomenclature nomenclature);
        Task<bool> UpdateNomenclatureAsync(Nomenclature nomenclature);
        Task<bool> DeleteNomenclatureAsync(int id);
    }
}
