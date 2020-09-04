using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    public static class SubscriberExtensions
    {
        // public static async Task<SubscriberModel> GetSubscriberAsync(this DatabaseContext context)
        // {
        //     return await context.Subscribers.FirstOrDefaultAsync();
        // }

        public static async Task<SubscriberAddress> GetAllSubscriberAddresses(this DatabaseContext context, SubscriberModel subscriber)
        {
            return await context.SubscriberAddresses.FirstOrDefaultAsync(sa => sa.SubscriberID == subscriber.Id);
        }

        public static async Task<string> FindSubscriberByMeter(this DatabaseContext context,string meterID)
        {
            var subscriber_meter = await context.SubscriberMeters.FirstOrDefaultAsync(sm => sm.MeterID == meterID);
            if(subscriber_meter == null)
                return null;
            return subscriber_meter.SubscriberID;
        }

        public static async Task<SubscriberModel> FindSubscriberByID(this DatabaseContext context,UserManager<SubscriberModel> userManger,string id)
        {
            return await userManger.FindByIdAsync(id);
        }
    }
}