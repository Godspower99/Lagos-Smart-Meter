namespace LagosSmartMeter
{
    // server-side methods
    public static class ServerMethods
    {
        public const string SetActiveMeter = "set-active-meter";
        public const string RefreshUserDetails = "refresh-user-details";
        public const string RefeshMeterDetails = "refresh-meter-details";
        public const string ActivateMeter = "activate-meter";
        public const string DeactivateMeter = "deactivate-meter";
        public const string SetMeterAlert = "set-meter-alert";
        public const string CancelMeterAlert = "cancel-meter-alert";
        public const string CancelAllMeterAlerts = "cancel-all-alerts";
    }

    // client-side methods
    public static class ClientMethods
    {
        public const string UpdateMeterReadings = "update-meter-readings";
        public const string UpdateMeterState = "update-meter-state";
        public const string UpdateUserDetails = "update-user-details";
        public const string UpdateMeterDetails = "update-meter-details";
        public const string PushAlertNotification = "receive-alert-notification";
    }
}