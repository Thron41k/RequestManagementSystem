using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfClient.Models.ExcelWriterModels;
using WpfClient.Services.Interfaces;

namespace WpfClient.Services.ExcelTemplate
{
    public class ActPartsTemplate : ExcelTemplateWriterBase<IEnumerable<ActPartsModel>>
    {
        public override ExcelTemplateType TemplateType => ExcelTemplateType.ActParts;

        public override byte[] FillTemplate(IEnumerable<ActPartsModel> data)
        {
            ExcelPackage.License.SetNonCommercialPersonal("Thron41k");
            using var stream = GetTemplateStream("ActPartsTemplate.xlsx");
            using var package = new ExcelPackage(stream);

            return package.GetAsByteArray();
        }
    }
}
