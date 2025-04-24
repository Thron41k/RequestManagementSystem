using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Models
{
    public class MaterialStock
    {
        public string ItemName { get; set; }
        public string Code { get; set; }
        public string Article { get; set; }
        public string Unit { get; set; }
        public double FinalBalance { get; set; }
    }
}
