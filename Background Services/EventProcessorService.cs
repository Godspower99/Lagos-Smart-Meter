using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace LagosSmartMeter
{
    public class EventProcessorService : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly EventProcessorHost _processor;
        public EventProcessorService(
            IConfiguration configuration)
        {
            // collect iothub eventhub processor host connection information
            this.configuration = configuration;
            var IoTHubName = configuration.GetValue<string>("EventProcessorHost:IoTHubName");
            var ConsumerGroupName = PartitionReceiver.DefaultConsumerGroupName;
            var HubConnectionString = configuration.GetValue<string>("EventProcessorHost:HubConnectionString");
            var StorageAccountConnectionString = configuration.GetValue<string>("EventProcessorHost:StorageAccountConnectionString");;
            var LeaseStorageContainerName = configuration.GetValue<string>("EventProcessorHost:LeaseStorageContainerName");;
            _processor = new EventProcessorHost(
                IoTHubName,
                ConsumerGroupName,
                HubConnectionString,
                StorageAccountConnectionString,
                LeaseStorageContainerName);
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // register telemetry processor
            await _processor.RegisterEventProcessorAsync<MeterTelemetryProcessor>();
            // wait until token cancellation request to unregister event processor
            while(!stoppingToken.IsCancellationRequested){ }
            await _processor.UnregisterEventProcessorAsync();
        }
    }

    public class MeterTelemetryProcessor : IEventProcessor
    {
        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Event Processor Service Stopped, Reason :: {reason.ToString()}");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"Event Processor Service Started On, Partition :: {context.PartitionId}");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Event Processor Service Error, error :: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            Console.WriteLine($"Processing events from Partition {context.PartitionId}");
            foreach (var message in messages)
            {
                // detect telemetry type
                if (message.Properties.TryGetValue("telemetry-type", out object value))
                {
                    var eventType = value as string;
                    // state-update telemetry
                    if (eventType == TelemetryType.readings.ToString())
                        {   
                            UpdateMeterReadings(message);
                            FilterMeterReadingForAlerts(message);
                        }
                    else if (eventType == TelemetryType.state.ToString())
                        UpdateMeterState(message);
                }
                context.CheckpointAsync(message).GetAwaiter();
            }
        }

        private async Task UpdateMeterState(EventData message)
        {
            // get eventdata body 
            var meterState = JsonConvert.DeserializeObject<Telemetry<MeterStateUpdate>>(
                Encoding.ASCII.GetString(message.Body));

            // device-id
            var deviceID = message.SystemProperties["iothub-connection-device-id"].ToString();
            // update meter state
            using (var context = new DatabaseContext())
            {
                // fetch meter from database
                var meter = await context.Meters.FirstOrDefaultAsync(m => m.MeterID == deviceID);
                var timeRecords = await context.MeterTimeRecords.FirstOrDefaultAsync(m => m.MeterID == deviceID);
                meter.MeterState = meterState.Body.State;

                // update time-records
                if (meterState.Body.State == MeterState.active.ToString())
                    timeRecords.LastActivation = meterState.SentTime;
                else if (meterState.Body.State == MeterState.standby.ToString())
                    timeRecords.LastDeactivation = meterState.SentTime;
                else if (meterState.Body.State == MeterState.shutting_down.ToString())
                    timeRecords.LastShutDown = meterState.SentTime;
                context.Meters.Update(meter);
                context.MeterTimeRecords.Update(timeRecords);
                await context.SaveChangesAsync();

                // set notification for meter state change
                await context.AddMeterNotification(new MeterNotification{
                    MeterID = meter.MeterID,
                    NotificationType = MeterNotificationType.MeterState.ToString(),
                    Time = meterState.SentTime,
                    Seen = false
                });
            }
        }

        private async Task UpdateMeterReadings(EventData message)
        {
            // get eventdata body
            var meterReadings = JsonConvert.DeserializeObject<Telemetry<MeterLatestReadings>>(
                Encoding.ASCII.GetString(message.Body)
            );

            // device-id
            var deviceID = message.SystemProperties["iothub-connection-device-id"].ToString();

            //Console.WriteLine($"{deviceID} :: {Encoding.ASCII.GetString(message.Body)}");
            // update meter readings
            using (var context = new DatabaseContext())
            {
                // fetch meter readings
                var readings = await context.MeterReadings.FirstOrDefaultAsync(m => m.MeterID == deviceID);

                // add new meter readings non-exists
                if(readings == null)
                {
                    var newReadings = new MeterLatestReadings{
                        MeterID = deviceID,
                        CurrentPower = meterReadings.Body.CurrentPower,
                        CurrentVoltage = meterReadings.Body.CurrentVoltage,
                        EnergyUsedInHours = meterReadings.Body.EnergyUsedInHours,
                        EnergyUsedInSeconds = meterReadings.Body.EnergyUsedInSeconds,
                        AvailableEnergy = meterReadings.Body.AvailableEnergy
                    };
                    await context.MeterReadings.AddAsync(newReadings);
                }
                else
                {
                    readings.CurrentPower = meterReadings.Body.CurrentPower;
                    readings.CurrentVoltage = meterReadings.Body.CurrentVoltage;
                    readings.EnergyUsedInSeconds = meterReadings.Body.EnergyUsedInSeconds;
                    readings.EnergyUsedInHours = meterReadings.Body.EnergyUsedInHours;
                    readings.LastUpdate = meterReadings.SentTime;
                    readings.AvailableEnergy = meterReadings.Body.AvailableEnergy;
                    context.MeterReadings.Update(readings);
                }
                await context.SaveChangesAsync();
                // set notification for meter state change
                await context.AddMeterNotification(new MeterNotification{
                   MeterID = readings.MeterID,
                   NotificationType = MeterNotificationType.MeterReadings.ToString(),
                   Time = readings.LastUpdate,
                   Seen = false
                });
                
            }
        }

        private async Task FilterMeterReadingForAlerts(EventData message)
        {
             // get eventdata body
            var meterReadings = JsonConvert.DeserializeObject<Telemetry<MeterLatestReadings>>(
                Encoding.ASCII.GetString(message.Body)
            );

            // device-id
            var deviceID = message.SystemProperties["iothub-connection-device-id"].ToString();
            using(var context = new DatabaseContext())
            {
               // handle power alerts
               var powerAlerts = await context.GetNewMeterAlerts(deviceID,MeterAlertType.Power.ToString());
               if(powerAlerts.Count > 0)
               {
                   // latest power alert
                   var powerAlert = powerAlerts.Dequeue();
                   if(meterReadings.Body.CurrentPower >= powerAlert.Limit)
                   {
                       // fetch subscriber for this meter
                       var subscriberID = await context.FindSubscriberByMeter(deviceID);
                       powerAlert.State = AlertState.Completed.ToString();

                       // notification content
                       var content = JsonConvert.SerializeObject(powerAlert);

                       // add notification to subscriber
                       await context.AddSubscriberNotification(new SubscriberNotification{
                           SubscriberID = subscriberID,
                           NotificationType = SubscriberNotificationsType.MeterAlert.ToString(),
                           Time = DateTime.UtcNow,
                           Sent = false,
                           Content = content
                       });

                       if(powerAlert.AutoDeactivate)
                        await context.NewFireAndForgetJob(new FireAndForgetMeterJob
                        {
                            MeterID = deviceID,
                            Job = MeterJobTypes.Deactivate.ToString(),
                            EnquedTime = DateTime.UtcNow,
                            Completed = false
                        });

                        // update all powerAlerts to completed
                        foreach(var alert in powerAlerts)
                            await context.CompleteMeterAlert(alert);
                   }
               }

               // handle voltage alerts
               var voltageAlerts = await context.GetNewMeterAlerts(deviceID,MeterAlertType.Voltage.ToString());
               if(voltageAlerts.Count > 0)
               {
                   // latest voltage alert
                   var voltageAlert = voltageAlerts.Dequeue();
                   if(meterReadings.Body.CurrentVoltage >= voltageAlert.Limit)
                   {
                        // fetch subscriber for this meter
                       var subscriberID = await context.FindSubscriberByMeter(deviceID);
                       voltageAlert.State = AlertState.Completed.ToString();

                       // notification content
                       var content = JsonConvert.SerializeObject(voltageAlert);

                       // add notification to subscriber
                       await context.AddSubscriberNotification(new SubscriberNotification{
                           SubscriberID = subscriberID,
                           NotificationType = SubscriberNotificationsType.MeterAlert.ToString(),
                           Time = DateTime.UtcNow,
                           Sent = false,
                           Content = content
                       });

                       if(voltageAlert.AutoDeactivate)
                        await context.NewFireAndForgetJob(new FireAndForgetMeterJob
                        {
                            MeterID = deviceID,
                            Job = MeterJobTypes.Deactivate.ToString(),
                            EnquedTime = DateTime.UtcNow,
                            Completed = false
                        });
                        
                        // update all voltage alerts to completed
                        foreach(var alert in voltageAlerts)
                            await context.CompleteMeterAlert(voltageAlert);
                   }
               }

               // handle energy alerts
               var energyAlerts = await context.GetNewMeterAlerts(deviceID,MeterAlertType.Energy.ToString());
               if(energyAlerts.Count > 0)
               {
                   // latest voltage alert
                   var energyAlert = energyAlerts.Dequeue();
                   if(meterReadings.Body.EnergyUsedInHours >= energyAlert.Limit)
                   {
                        // fetch subscriber for this meter
                       var subscriberID = await context.FindSubscriberByMeter(deviceID);
                       energyAlert.State = AlertState.Completed.ToString();

                       // notification content
                       var content = JsonConvert.SerializeObject(energyAlert);

                       // add notification to subscriber
                       await context.AddSubscriberNotification(new SubscriberNotification{
                           SubscriberID = subscriberID,
                           NotificationType = SubscriberNotificationsType.MeterAlert.ToString(),
                           Time = DateTime.UtcNow,
                           Sent = false,
                           Content = content
                       });

                       if(energyAlert.AutoDeactivate)
                        await context.NewFireAndForgetJob(new FireAndForgetMeterJob
                        {
                            MeterID = deviceID,
                            Job = MeterJobTypes.Deactivate.ToString(),
                            EnquedTime = DateTime.UtcNow,
                            Completed = false
                        });
                        // update all voltage alerts to completed
                        foreach(var alert in energyAlerts)
                            await context.CompleteMeterAlert(energyAlert);
                   }
               }

               // TODO :: ADD COST ALERT AFTER BILLING HAS BEEN ADDED TO CODE
            }
        }
    }
}