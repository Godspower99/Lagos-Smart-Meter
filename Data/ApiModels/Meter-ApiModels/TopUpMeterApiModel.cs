namespace LagosSmartMeter
{
    public class TopUpMeterRequestApiModel
    {
        public string MeterID { get; set; }
        public string Token { get; set; }
    }

    public class TopUpMeterResponseApiModel
    {
        public double Value { get; set; }
    }
}