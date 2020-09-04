using System.ComponentModel.DataAnnotations;

public class MeterTariff
{   
    [Key]
    public string TarrifID { get; set; }
    public string Name { get; set; }
    public double KwhPrice { get; set; }
    public int AccessRatePeriod { get; set; }
    public double AccessRatePrice { get; set; }
    public bool HasAccessRate { get; set; }
}