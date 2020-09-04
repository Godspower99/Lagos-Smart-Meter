using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace LagosSmartMeter
{
    public class SubscriberHub : Hub
    {
        // userManager for subscriber interactions
        private readonly UserManager<SubscriberModel> userManager;
        private readonly IConfiguration configuration;
        private DeviceService deviceService;
        private CancellationTokenSource cancellationTokenSource;
        private string SubscriberID = "";
        public SubscriberHub(
            UserManager<SubscriberModel> userManager,
            IConfiguration configuration,
            DeviceService deviceService)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.deviceService = deviceService;
            this.cancellationTokenSource = new CancellationTokenSource();
        }

    // public override Task OnConnectedAsync()
    // {
    //     base.OnConnectedAsync();
    //    // var user = userManager.GetUserAsync(Context.User).GetAwaiter().GetResult();
    //     SubscriberID = "";
    //     return Task.CompletedTask;
    // }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        base.OnDisconnectedAsync(exception);
        cancellationTokenSource.Cancel();
        return Task.CompletedTask;
    }
    
    #region Helper-Methods

    private async Task UpdateClientMeterDetails(string meterID)
    {
        using(var context = new DatabaseContext())
        {
            // fetch user active meter
            var meter = await context.FindMeterByID(meterID);
            if(meter != null)
            {
                // get meter type details
                var meterType = await context.GetMeterTypeAsync(meter);
                // get meter tariff plan
                var meterTariff = await context.GetMeterTariffAsync(meter);
                var readings = await context.GetMeterLatestReadingsAsync(meter);
                // update meter details
                await Clients.Caller.SendAsync(ClientMethods.UpdateMeterDetails,new {
                    MeterID = meter.MeterID,
                    MeterType = meterType.Name,
                    MeterTariff = meterTariff.Name,
                });
            }
        }
    }
    private async Task UpdateMeterState(MeterModel meter,IClientProxy caller)
     {    
        await caller.SendAsync(ClientMethods.UpdateMeterState,new {
            MeterID = meter?.MeterID,
            State = meter?.MeterState
        });
     }
    private async Task UpdateMeterReadings(MeterLatestReadings readings,IClientProxy caller)
    {
        await caller.SendAsync(ClientMethods.UpdateMeterReadings,new {
                MeterID = readings?.MeterID,
                CurrentPower = readings?.CurrentPower,
                CurrentVoltage = readings?.CurrentVoltage,
                Energy = readings?.EnergyUsedInHours 
                });
    }
    private async Task UpdateMeterReadingsTask(CancellationToken token,string meterID,IClientProxy caller)
    {
        while(!token.IsCancellationRequested)
        {
            using(var context = new DatabaseContext())
            {
                // check for new meter readings notifications
                var newNotifications = await context.GetNewMeterNotifications(meterID,MeterNotificationType.MeterReadings.ToString());
                if(newNotifications.LongCount() > 0)
                {
                    // update meter client with latest readings
                    var meter = await context.FindMeterByID(meterID);
                    var meterReadings = await context.GetMeterLatestReadingsAsync(meter);
                    await UpdateMeterReadings(meterReadings,caller);
                    foreach(var notification in newNotifications)
                        await context.SeenMeterNotification(notification);
                }
            }
        }
    }
    private async Task UpdateMeterStateTask(CancellationToken token,string meterID,IClientProxy caller)
    {
        while(!token.IsCancellationRequested)
        {
            using(var context = new DatabaseContext())
            {
                // check for new meter state changed notifications
                var newNotifications = await context.GetNewMeterNotifications(meterID,MeterNotificationType.MeterState.ToString());
                if(newNotifications.LongCount() > 0)
                {
                    // update client with latest meter state value
                    var meter = await context.FindMeterByID(meterID);
                    await UpdateMeterState(meter,caller);
                    foreach(var notification in newNotifications)
                        await context.SeenMeterNotification(notification);
                }
            }
        }
    }

    private async Task PushAlertNotificationToSubscriber(CancellationToken token,string subscriberID,IClientProxy caller)
    {
        while(!token.IsCancellationRequested)
        {
            using(var context = new DatabaseContext())
            {
                // handle meter alerts
                var newAlertNotifications = await context.GetNewSubscriberNotifications(subscriberID,SubscriberNotificationsType.MeterAlert.ToString());
                if(newAlertNotifications.LongCount() > 0)
                {
                    // invert list to send oldest notification first
                    newAlertNotifications = new Queue<SubscriberNotification>(newAlertNotifications.Reverse());
                    foreach(var notification in newAlertNotifications)
                    {
                        // deserialize notification content
                        var content = JsonConvert.DeserializeObject<MeterAlert>(notification.Content);
                        await caller.SendAsync(ClientMethods.PushAlertNotification,new {
                            MeterID = content.MeterID,
                            Type = content.Type,
                            SetTime = content.Time,
                            Time = notification.Time,
                            Limit = content.Limit,
                        });
                    }
                    // update notifications to sent
                    foreach(var notification in newAlertNotifications)
                       await context.SentSubscriberNotification(notification);
                }
            }
        }
    }

    #endregion

    #region SignalR Server methods
    [HubMethodName(ServerMethods.SetActiveMeter)]
    public async Task SetClientActiveMeterView(string meterID,string subscriberID)
    {
        Console.WriteLine("Initialize");
        // check if meter exists and belongs to user
        using(var context = new DatabaseContext())
        {    
            // fetch user and meter details 
            var user = await context.FindSubscriberByID(userManager,subscriberID);
            var metersub = await context.SubscriberMeters.FirstOrDefaultAsync(m => m.MeterID == meterID && m.SubscriberID == user.Id);
            if(metersub != null)
            {   
                // cancel any pre-existing tasks for updating meter details
                cancellationTokenSource.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                var meter = await context.Meters.FirstOrDefaultAsync(m => m.MeterID == metersub.MeterID);
                var readings = await context.GetMeterLatestReadingsAsync(meter);
                await UpdateClientMeterDetails(metersub.MeterID);
                await UpdateMeterState(meter,Clients.Caller);
                await UpdateMeterReadings(readings,Clients.Caller);
                UpdateMeterStateTask(cancellationTokenSource.Token,meter.MeterID,Clients.Caller);
                UpdateMeterReadingsTask(cancellationTokenSource.Token,meter.MeterID,Clients.Caller);
                PushAlertNotificationToSubscriber(cancellationTokenSource.Token,user.Id,Clients.Caller);
            }
        }
    }

    [HubMethodName(ServerMethods.RefreshUserDetails)]
    public async Task RefreshSubscriberDetails(string subscriberID)
    {
        using(var context = new DatabaseContext())
        {
            // fetch user from usermanager
            var user = await context.FindSubscriberByID(userManager,subscriberID);
            var address = await context.GetAllSubscriberAddresses(user);
            var meters = await context.FindMetersBySubscriberIDAsync(user);
            var details = new {
                Name = user.FullName,
                Email = user.Email,
                Birthday = user.Birthday,
                Gender = user.Gender,
                Address = new Address{
                    city = address.city,
                    Street = address.Street,
                    BuildingNumber = address.BuildingNumber,
                    Phone = address.Phone
                },
                Meters = meters,
            };
            await Clients.Caller.SendAsync(ClientMethods.UpdateUserDetails,details);
        }
    }
    
    [HubMethodName(ServerMethods.ActivateMeter)]
    public async Task ActivateMeter(string meterID,string subscriberID)
    {
        using(var context = new DatabaseContext())
        {    
            // fetch user and meter details 
            var user = await context.FindSubscriberByID(userManager,subscriberID);
            var metersub = await context.SubscriberMeters.FirstOrDefaultAsync(m => m.MeterID == meterID && m.SubscriberID == user.Id);
            if(metersub != null)
                await deviceService.ActivateDevice(configuration,meterID);   
        }
    }

    [HubMethodName(ServerMethods.DeactivateMeter)]
    public async Task DeactivateMeter(string meterID,string subscriberID)
    {
        using(var context = new DatabaseContext())
        {    
            // fetch user and meter details 
            var user = await context.FindSubscriberByID(userManager,subscriberID);
            var metersub = await context.SubscriberMeters.FirstOrDefaultAsync(m => m.MeterID == meterID && m.SubscriberID == user.Id);
            if(metersub != null)
                await deviceService.DeactivateDevice(configuration,meterID);   
        }
    }

    [HubMethodName(ServerMethods.SetMeterAlert)]
    public async Task SetMeterAlert(SetMeterAlertParameter alert)
    {
        using(var context = new DatabaseContext())
        {
            await context.AddMeterAlert(new MeterAlert{
                MeterID = alert.MeterID,
                AutoDeactivate = alert.AutoDeactivate,
                Limit = alert.Limit,
                State = AlertState.Pending.ToString(),
                Time = DateTime.UtcNow,
                Type = alert.AlertType
            });
        }
    } 

    [HubMethodName(ServerMethods.CancelMeterAlert)]
    public async Task CancelMeterAlert(CancelMeterAlertParameter cancelAlert)
    {
        using(var context = new DatabaseContext())
        {
            var alerts = await context.GetNewMeterAlerts(cancelAlert.MeterID,cancelAlert.AlertType);
            foreach(var alert in alerts)
                await context.CancelMeterAlert(alert);
        }
    }

    [HubMethodName(ServerMethods.CancelAllMeterAlerts)]
    public async Task CancelAllMeterAlerts(string meterID)
    {
        using(var context = new DatabaseContext())
        {
            await context.CancelAllMeterAlerts(meterID);
        }
    }

    #endregion
}

#region Subscriber hub method custom parameters

public class SetMeterAlertParameter
{
    public string MeterID { get; set; }
    public string AlertType { get; set; }
    public double Limit { get; set; }
    public bool AutoDeactivate { get; set; }
}

public class CancelMeterAlertParameter
{
    public string MeterID { get; set; }
    public string AlertType { get; set; }
}
#endregion
}