using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class MeterToken
    {   
        [Key]
        public int ID { get; set; }
        public string TokenID { get; set; }
        public string MeterID { get; set; }
        public double Value { get; set; }
        public bool Used { get; set; }
        public DateTime CreationTime { get; set; }
        public string SubscriberID { get; set; }
    }
}