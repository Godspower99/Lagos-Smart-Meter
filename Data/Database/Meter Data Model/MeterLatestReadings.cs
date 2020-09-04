using System;
using System.ComponentModel.DataAnnotations;

public class MeterLatestReadings
{   
    [Key]
    public string MeterID { get; set; }
    public double CurrentPower { get; set;}
    public double CurrentVoltage { get; set; }
    public double EnergyUsedInSeconds { get; set; }
    public double EnergyUsedInHours { get; set; }
    public DateTime LastUpdate { get; set; }
    public double AvailableEnergy { get; set; }
}