using System;

public class MedicamentoNoControladoPdfVM
{
    public string ProductoNombre { get; set; }
    public decimal UnidadesIniciales { get; set; }
    public decimal UnidadesExtra { get; set; }
    public decimal Utilizado { get; set; }
    public decimal Descartado { get; set; }
    public decimal Retornadas { get; set; }
    public DateTime FechaProcedimiento { get; set; }
}