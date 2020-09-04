namespace LagosSmartMeter
{
    public static class ErrorCodes
    {
        /// <summary>
        /// invalid request body model
        /// </summary>
        public static int InvalidRequestBody = 441;
        
        /// <summary>
        /// certificate error
        /// </summary>
        public static int InvalidCertificate = 442;

        /// <summary>
        /// registration attempt failure 
        /// not because of meter credentials
        /// </summary>
        public static int RegistrationAttemptFailure = 443;

        public static int TopUpFailed = 445;
    }
}