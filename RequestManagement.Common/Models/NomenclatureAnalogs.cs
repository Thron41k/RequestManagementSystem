namespace RequestManagement.Common.Models;

public class NomenclatureAnalog
{
    public int Id { get; set; }
    public int MainNomenclatureId { get; set; }    // Основная номенклатура
    public int AnalogNomenclatureId { get; set; }  // Аналог номенклатуры
    public Nomenclature MainNomenclature { get; set; }
    public Nomenclature AnalogNomenclature { get; set; }
}