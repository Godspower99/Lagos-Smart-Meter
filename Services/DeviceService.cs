using System;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;

namespace LagosSmartMeter
{
    public class DeviceService
    {
        public readonly ServiceClient ServiceClient;
        private readonly IConfiguration _configuration;
        public DeviceService(IConfiguration configuration)
        { 
            _configuration = configuration;
            var connectionString = configuration.GetValue<string>("DeviceService:ConnectionString");
            ServiceClient = ServiceClient.CreateFromConnectionString(connectionString);
        }
    }

    public static class DeviceServiceExtensions
    {
        public static async Task<bool> ActivateDevice(this DeviceService service,IConfiguration configuration,string deviceID)
        {
            string activateMethod = configuration.GetValue<string>("DeviceMethods:Activate");
            var cloudToDeviceMethod = new CloudToDeviceMethod(activateMethod,TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(10));
            var result = await service.ServiceClient.InvokeDeviceMethodAsync(deviceID,cloudToDeviceMethod);
            if(result != null)
                if(result.Status == 200)
                    return true;
            return false;
        }

        public static async Task<bool> DeactivateDevice(this DeviceService service,IConfiguration configuration,string deviceID)
        {
            string deactivateMethod = configuration.GetValue<string>("DeviceMethods:Deactivate");
            var cloudToDeviceMethod = new CloudToDeviceMethod(deactivateMethod,TimeSpan.FromSeconds(10),TimeSpan.FromSeconds(10));
            var result = await service.ServiceClient.InvokeDeviceMethodAsync(deviceID,cloudToDeviceMethod);
            if(result != null)
                if(result.Status == 200)
                    return true;
            return false;
        }
    }
}