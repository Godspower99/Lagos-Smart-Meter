using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    public static class NotificationExtensions
    {
        public static async Task<Queue<MeterNotification>> GetNewMeterNotifications(
            this DatabaseContext context,string meterID,string notificationType)
        {
            // get all new notifications
            var notifications = await context.MeterNotifications
                .Where(mn => mn.MeterID == meterID && 
                        mn.NotificationType == notificationType &&
                        mn.Seen == false).ToListAsync();    
            // arrange by time in descending order
            return new Queue<MeterNotification>(notifications.OrderBy(s => s.Time));
        }

        public static async Task<MeterNotification> AddMeterNotification(
            this DatabaseContext context, MeterNotification meterNotification)
        {
            await context.AddAsync(meterNotification);
            await context.SaveChangesAsync();
            return meterNotification;
        }

        public static async Task CleanMeterNotifications(this DatabaseContext context)
        {
            var notifications = await context.MeterNotifications.Where(mn => mn.Seen == true).ToListAsync();
            context.MeterNotifications.RemoveRange(notifications);
            await context.SaveChangesAsync();
        }

        public static async Task SeenMeterNotification(this DatabaseContext context,MeterNotification notification)
        {
            var seenNotification = await context.MeterNotifications.FirstOrDefaultAsync(mn => mn.ID == notification.ID);
            notification.Seen = true;
            context.MeterNotifications.Update(seenNotification);
            await context.SaveChangesAsync();
        }

        public static async Task<Queue<SubscriberNotification>> GetNewSubscriberNotifications(
            this DatabaseContext context,string subscriberID,string notificationType)
        {
            // get new notifications
            var notifications = await context.SubscriberNotifications
                .Where(sn => sn.SubscriberID == subscriberID && 
                        sn.NotificationType == notificationType &&
                        sn.Sent == false).ToListAsync();    
            // arrange by time in descending order
            return new Queue<SubscriberNotification>(notifications.OrderBy(n => n.Time));
        }

        public static async Task CleanSubscriberNotifications(this DatabaseContext context)
        {
            var notifications = await context.SubscriberNotifications.Where(sn => sn.Sent == true).ToListAsync();
            context.SubscriberNotifications.RemoveRange(notifications);
            await context.SaveChangesAsync();
        }

        public static async Task SentSubscriberNotification(this DatabaseContext context,SubscriberNotification notification)
        {
            var sentNotification = await context.SubscriberNotifications.FirstOrDefaultAsync(sn => sn.ID == notification.ID);
            sentNotification.Sent = true;
            context.SubscriberNotifications.Update(sentNotification);
            await context.SaveChangesAsync();
        }

        public static async Task<SubscriberNotification> AddSubscriberNotification(
            this DatabaseContext context, SubscriberNotification notification)
        {
            await context.AddAsync(notification);
            await context.SaveChangesAsync();
            return notification;
        }
    }
}