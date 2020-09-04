using System.ComponentModel.DataAnnotations;
public class MeterLocation
{  
    [Key]
    public string MeterID { get; set; }
    public string city { get; set; }
    public string Phone { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
}