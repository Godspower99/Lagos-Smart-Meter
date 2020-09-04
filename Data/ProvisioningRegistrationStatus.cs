namespace LagosSmartMeter
{
    //
    // Summary:
    //     The provisioning status type.
    public enum ProvisioningRegistrationStatus
    {
        //
        // Summary:
        //     Device has not yet come on-line
        Unassigned = 1,
        //
        // Summary:
        //     Device has connected to the DRS but IoT Hub ID has not yet been returned to the
        //     device
        Assigning = 2,
        //
        // Summary:
        //     DRS successfully returned a device ID and connection string to the device
        Assigned = 3,
        //
        // Summary:
        //     Device enrollment failed
        Failed = 4,
        //
        // Summary:
        //     Device is disabled
        Disabled = 5
    }
}