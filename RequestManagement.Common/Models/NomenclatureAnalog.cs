using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models.Interfaces;

namespace RequestManagement.Common.Models
{
    public class NomenclatureAnalog : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Nomenclature Original { get; set; }
        public int OriginalId { get; set; }
        public Nomenclature Analog { get; set; }
        public int AnalogId { get; set; }
    }
}
