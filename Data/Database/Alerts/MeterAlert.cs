using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class MeterAlert
    {
        [Key]
        public int ID { get; set; }
        public double Limit { get; set; }
        public string MeterID { get; set; }
        public bool AutoDeactivate { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }

    public enum MeterAlertType
    {
        Energy,
        Power,
        Voltage,
        Cost
    }

    public enum AlertState
    {
        Pending,
        Completed,
        Cancelled
    }
}