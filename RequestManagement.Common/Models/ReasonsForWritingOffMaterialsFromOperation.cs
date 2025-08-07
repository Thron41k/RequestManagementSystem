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
    public class ReasonsForWritingOffMaterialsFromOperation : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Reason { get; set; } = "";
    }
}
