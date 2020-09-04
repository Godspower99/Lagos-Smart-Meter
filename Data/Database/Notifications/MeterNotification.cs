using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class MeterNotification
    {  
        [Key]
        public int ID { get; set; }
        public string MeterID { get; set; }
        public string NotificationType { get; set; }
        public DateTime Time { get; set; } 
        public bool Seen { get; set; }
    }

    public enum MeterNotificationType
    {
        MeterReadings,
        MeterState,
    }
}