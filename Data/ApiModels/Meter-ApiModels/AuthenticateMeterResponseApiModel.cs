using System;
using Microsoft.Azure.Devices.Provisioning.Client;

namespace LagosSmartMeter
{
    public class AuthenticateMeterResponseApiModel
    {
        public string DeviceID { get; set; }
        public string AssignedIoTHub { get; set; }
        public MeterLastEnergyReadings MeterLastEnergyReadings { get; set; }
    }

    public class MeterLastEnergyReadings
    {
        public double UsedEnergyInSeconds { get; set; }
        public double UsedEnergyInHours { get; set; }
        public double AvailableEnergy { get; set; }
    }
}