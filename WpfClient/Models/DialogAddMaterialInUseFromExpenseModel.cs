using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RequestManagement.Common.Models;

namespace RequestManagement.WpfClient.Models
{
    public class DialogAddMaterialInUseFromExpenseModel
    {
        public Driver? Mol { get; set; }
        public Guid Caller { get; set; }
        public bool Result { get; set; }
    }
}
