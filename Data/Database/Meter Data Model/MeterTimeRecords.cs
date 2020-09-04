using System;
using System.ComponentModel.DataAnnotations;

public class MeterTimeRecords
{
        [Key]
        public string MeterID { get; set; }
        public DateTime InitialActivation { get; set; }

        public DateTime LastActivation { get; set; }

        public DateTime LastDeactivation { get; set; }

        public DateTime LastShutDown { get; set; }

        public DateTime LastStartUp { get; set; }

        public DateTime LastUpdate { get; set; }

}