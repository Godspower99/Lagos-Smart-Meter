using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class SubscriberLoginCredentials
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class SubscriberLoginResponse
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        // NOTE ADD LATER 
        //public List<string> Meters { get; set; }
        public string MeterID { get; set; }
    }

    public class Address 
    {
        public string city { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string BuildingNumber { get; set; }
    }
}