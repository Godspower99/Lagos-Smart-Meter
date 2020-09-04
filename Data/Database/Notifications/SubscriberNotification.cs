using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter 
{
    public class SubscriberNotification
    {
        [Key]
        public int ID { get; set; }
        public string SubscriberID { get; set; }
        public string NotificationType { get; set;}
        public bool Sent { get; set; }
        public DateTime Time { get; set; }
        public string Content { get; set; }
    }

    public enum SubscriberNotificationsType
    {
        MeterAlert,
        MeterSchedule
    }
}