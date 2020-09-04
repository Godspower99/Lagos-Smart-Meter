using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class FireAndForgetMeterJob
    {
        [Key]
        public int ID { get; set; }
        public bool Completed { get; set; }
        public string MeterID { get; set; }
        public string Job { get; set; }
        public DateTime EnquedTime { get; set; }
    }

    public enum MeterJobTypes
    {
        Activate,
        Deactivate
    }
}