using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class DefectGroup
    {
        public int Id { get; set; }
        public string Name { get; set; }          // Название
        public List<Defect> Defects { get; set; } // Деффекты в группе
    }
}
