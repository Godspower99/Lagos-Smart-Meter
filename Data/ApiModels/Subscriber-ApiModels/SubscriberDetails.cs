using System;
using System.Collections.Generic;

public class SubscriberDetails
{
    public string FullName { get; set; }
    public string EmailAddress { get; set; }
    public List<SubscriberAddressDetails> Addresses { get; set;}
    public SubscriberMeterDetails Meter { get; set; }
    public MeterTariffDetails Tariff { get; set; }


}

public class SubscriberAddressDetails
{
    public string city { get; set; }
    public string Phone { get; set; }
    public string Street { get; set; }
    public string BuildingNumber { get; set; }
}

public class SubscriberMeterDetails
{    public DateTime InitialActivation { get; set; }
    public DateTime LastActivation { get; set; }
    public DateTime LastDeactivation { get; set; }
    public DateTime LastShutDown { get; set; }
    public DateTime LastStartUp { get; set; }
    public DateTime LastUpdate { get; set; }
    public string MeterType { get; set; }
    public string MeterTypeDescription { get; set;}
    public string MeterID { get; set; }
    public string MeterState { get; set; }
}

public class MeterTariffDetails
{
    public string TariffName { get; set; }
    public double KwhPrice { get; set; }
    public int AccessRatePeriod { get; set; }
    public double AccessRatePrice { get; set; }
    public bool HasAccessRate { get; set; }
}
