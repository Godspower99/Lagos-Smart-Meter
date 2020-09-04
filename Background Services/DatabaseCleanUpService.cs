using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace LagosSmartMeter
{
    public class DatabaseCleanUpService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // continue until cancellation is requested
            while(!stoppingToken.IsCancellationRequested)
            {
                using(var context = new DatabaseContext())
                {
                    // remove all seen meter notification
                    await context.CleanMeterNotifications();
                    // remove all sent subscriber notifications
                    await context.CleanSubscriberNotifications();
                    // purge completed and cancelled alerts
                    await context.PurgeMeterAlerts();
                    // clean completer jobs
                    await context.CleanFireAndForgetJobs();

                    // run again afte 60 seconds
                    await Task.Delay(1000 * 60); 
                }
            }
        }
    }
}