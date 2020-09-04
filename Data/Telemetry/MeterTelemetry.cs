using System;

namespace LagosSmartMeter
{
public class Telemetry<T>
{
    public T Body { get; set; }
    public DateTime SentTime { get; set; }
}

public enum TelemetryType
{
    readings,
    state,
}

public class MeterReadings
{
    public double CurrentPower { get; set; }
    public double EnergyUsedInSeconds { get; set; }
    public double EnergyUsedInHours { get; set; }
    public double CurrentVoltageReading { get; set; }
}

public enum MeterState
{
    active,
    standby,
    shutting_down,
    unknown
}

public class MeterStateUpdate
{
    public string State { get; set; }
}
}
