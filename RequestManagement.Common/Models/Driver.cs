using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class Driver
    {
        public int Id { get; set; }
        public string FullName { get; set; }         // Полное ФИО
        public string ShortName { get; set; }        // Фамилия с инициалами
        public string Position { get; set; }         // Должность
    }
}
