using RequestManagement.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Interfaces
{
    public interface IEquipmentService
    {
        Task<int> CreateEquipmentAsync(Equipment equipment);
        Task<bool> UpdateEquipmentAsync(Equipment equipment);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<List<Equipment>> GetAllEquipmentAsync(string filter = "");
    }
}
