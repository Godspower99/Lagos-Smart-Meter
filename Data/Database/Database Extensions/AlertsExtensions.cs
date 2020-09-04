using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    public static class AlertsExtensions
    {
        public static async Task<Queue<MeterAlert>> GetNewMeterAlerts(this DatabaseContext context,string meterID,string alertType)
        {
            // get all new notifications
            var alert = await context.MeterAlerts
                .Where(ma => ma.MeterID == meterID && 
                        ma.Type == alertType &&
                        ma.State == AlertState.Pending.ToString()).ToListAsync();    
            // arrange by time in descending order
            return new Queue<MeterAlert>(alert.OrderByDescending(ma => ma.Time));
        }
        public static async Task<MeterAlert> AddMeterAlert(this DatabaseContext context, MeterAlert alert)
        {
            await context.AddAsync(alert);
            await context.SaveChangesAsync();
            return alert;
        }
        public static async Task PurgeMeterAlerts(this DatabaseContext context)
        {
            var alerts = await context.MeterAlerts.Where(ma => ma.State == AlertState.Completed.ToString() ||
                                                        ma.State == AlertState.Cancelled.ToString()).ToListAsync();
            context.MeterAlerts.RemoveRange(alerts);
            await context.SaveChangesAsync();
        }
        public static async Task CompleteMeterAlert(this DatabaseContext context,MeterAlert alert)
        {
            var completedAlert = await context.MeterAlerts.FirstOrDefaultAsync(ma => ma.ID == ma.ID);
            completedAlert.State = AlertState.Completed.ToString();
            context.MeterAlerts.Update(completedAlert);
            await context.SaveChangesAsync();
        }

        public static async Task CancelMeterAlert(this DatabaseContext context,MeterAlert alert)
        {
            var cancelledAlert = await context.MeterAlerts.FirstOrDefaultAsync(ma => ma.ID == ma.ID);
            cancelledAlert.State = AlertState.Cancelled.ToString();
            context.MeterAlerts.Update(cancelledAlert);
            await context.SaveChangesAsync();
        }

        public static async Task CancelAllMeterAlerts(this DatabaseContext context,string meterID)
        {
            var alerts = await context.MeterAlerts.Where(a => a.MeterID == meterID && a.State == AlertState.Pending.ToString()).ToListAsync();
            foreach(var alert in alerts)
                await context.CancelMeterAlert(alert);
        }
    }
}