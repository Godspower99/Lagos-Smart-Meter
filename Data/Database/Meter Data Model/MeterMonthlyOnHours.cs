using System.ComponentModel.DataAnnotations;

public class MeterMonthlyOnHours
{
    [Key]
    public int ID { get; set; }
    public string MeterID { get; set; }
    public int Year { get; set;}
    public int Month { get; set;}
    public int OnHours { get; set; }
}