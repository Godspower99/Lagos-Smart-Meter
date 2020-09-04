using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace LagosSmartMeter
{
    public class FireForgetJobsExecutorService : BackgroundService
    {
        private readonly DeviceService deviceService;
        private readonly IConfiguration configuration;
        public FireForgetJobsExecutorService(
            DeviceService deviceService,
            IConfiguration configuration)
        {
            this.deviceService = deviceService;
            this.configuration = configuration;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                using(var context = new DatabaseContext())
                {
                    // get all new fire and forget jobs
                    var newJobs = await context.GetNewFireAndForgetJobs();
                    foreach(var job in newJobs)
                    { 
                        ExecuteMeterJob(job);
                        await context.CompleteFireAndForgetJob(job);
                    }
                }  
        
            }
        
        }

        private async Task ExecuteMeterJob(FireAndForgetMeterJob job)
        {
            // activate meter if requested by job
            if(job.Job == MeterJobTypes.Activate.ToString())
            {
                await deviceService.ActivateDevice(configuration,job.MeterID);
            }
            else if(job.Job == MeterJobTypes.Deactivate.ToString())
            {
                await deviceService.DeactivateDevice(configuration,job.MeterID);
            }
        }
    }
}