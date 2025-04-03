using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class Defect
    {
        public int Id { get; set; }
        public string Name { get; set; }            // Название
        public int DefectGroupId { get; set; }      // Группа дефекта (внешний ключ)
        public DefectGroup DefectGroup { get; set; } // Навигационное свойство
    }
}
