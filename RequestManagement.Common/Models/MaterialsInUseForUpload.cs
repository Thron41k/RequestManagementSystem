using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestManagement.Common.Models
{
    public class MaterialsInUseForUpload
    {
        public MaterialsInUseForUpload(){}

        public MaterialsInUseForUpload(MaterialsInUseForUpload materialsInUseForUpload)
        {
            DocumentNumber = materialsInUseForUpload.DocumentNumber;
            Date = materialsInUseForUpload.Date;
            Quantity = materialsInUseForUpload.Quantity;
            NomenclatureCode = materialsInUseForUpload.NomenclatureCode;
            NomenclatureName = materialsInUseForUpload.NomenclatureName;
            NomenclatureArticle = materialsInUseForUpload.NomenclatureArticle;
            NomenclatureUnitOfMeasure = materialsInUseForUpload.NomenclatureUnitOfMeasure;
            EquipmentName = materialsInUseForUpload.EquipmentName;
            EquipmentCode = materialsInUseForUpload.EquipmentCode;
            FinanciallyResponsiblePersonFullName = materialsInUseForUpload.FinanciallyResponsiblePersonFullName;
            FinanciallyResponsiblePersonCode = materialsInUseForUpload.FinanciallyResponsiblePersonCode;
        }
        public string DocumentNumber { get; set; }
        public string Date { get; set; }
        public decimal Quantity { get; set; }
        public string NomenclatureCode { get; set; }
        public string NomenclatureName { get; set; }
        public string NomenclatureArticle { get; set; }
        public string NomenclatureUnitOfMeasure { get; set; }
        public string EquipmentName { get; set; }
        public string EquipmentCode { get; set; }
        public string FinanciallyResponsiblePersonFullName { get; set; }
        public string FinanciallyResponsiblePersonCode { get; set; }
    }
}
