using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Models.ExcelWriterModels;

namespace WpfClient.Services.ExcelTemplate
{
    public class RequisitionInvoiceTemplate : ExcelTemplateWriterBase<IncomingPrintModel>
    {
        public override ExcelTemplateType TemplateType => ExcelTemplateType.RequisitionInvoice;

        public override byte[] FillTemplate(IncomingPrintModel data)
        {
            throw new NotImplementedException();
        }
    }
}
