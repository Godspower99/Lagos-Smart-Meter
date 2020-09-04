namespace LagosSmartMeter
{
    public class ResponseApiModel
    {
        public string ErrorMessage { get; set; }
        public bool IsSuccessful => ErrorMessage == null;
    }

    public class ResponseApiModel<T> : ResponseApiModel
    {
        public T Body { get; set; }
    }
}