using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    public static class JobExtensions
    {
        public static async Task<FireAndForgetMeterJob> NewFireAndForgetJob
        (this DatabaseContext context, FireAndForgetMeterJob job)
        {
            await context.AddAsync(job);
            await context.SaveChangesAsync();
            return job;
        }

        public static async Task CleanFireAndForgetJobs(this DatabaseContext context)
        {
            var completedJobs = await context.FireAndForgetMeterJobs.Where(j => j.Completed == true).ToListAsync();
            context.FireAndForgetMeterJobs.RemoveRange(completedJobs);
        }

        public static async Task CompleteFireAndForgetJob(this DatabaseContext context, FireAndForgetMeterJob job)
        {
            var completedJob = await context.FireAndForgetMeterJobs.FirstOrDefaultAsync(j => j.ID == job.ID);
            completedJob.Completed = true;
            context.FireAndForgetMeterJobs.Update(completedJob);
            await context.SaveChangesAsync();
        }

        public static async Task<Queue<FireAndForgetMeterJob>> GetNewFireAndForgetJobs(this DatabaseContext context)
        {
            var newJobs = await context.FireAndForgetMeterJobs.Where(j => j.Completed == false).ToListAsync();
            return new Queue<FireAndForgetMeterJob>(newJobs.OrderBy(j => j.EnquedTime));
        }
    }
}