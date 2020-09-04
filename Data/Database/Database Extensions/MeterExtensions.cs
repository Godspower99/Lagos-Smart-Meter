using System.Net.Mime;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LagosSmartMeter
{
    public static class MeterDatabaseExtenions
    {
        #region HelperMethods

        public static MeterModel UpdateMeterProperties(MeterModel originalMeter, MeterModel meterUpdate)
        {
            originalMeter.MeterState = meterUpdate.MeterState;
            originalMeter.ProvisionState = meterUpdate.ProvisionState;
            originalMeter.MeterState = meterUpdate.MeterState;
            originalMeter.TarriffID = meterUpdate.TarriffID;
            originalMeter.TypeID = meterUpdate.TypeID;
            return originalMeter;
        }

        public static MeterTimeRecords UpdateMeterTimeProperties(MeterTimeRecords originalTimeRecords, MeterTimeRecords timeRecordsUpdate)
        {
            originalTimeRecords.InitialActivation = timeRecordsUpdate.InitialActivation;
            originalTimeRecords.LastActivation = timeRecordsUpdate.LastActivation;
            originalTimeRecords.LastDeactivation = timeRecordsUpdate.LastDeactivation;
            originalTimeRecords.LastShutDown = timeRecordsUpdate.LastShutDown;
            originalTimeRecords.LastStartUp = timeRecordsUpdate.LastUpdate;
            originalTimeRecords.LastUpdate = timeRecordsUpdate.LastUpdate;

            return originalTimeRecords;
        }
        #endregion

        /// <summary>
        /// returns true if a specific meter exists in database
        /// </summary>
        /// <param name="meters"></param>
        /// <param name="meterId"></param>
        /// <returns></returns>
        public static async Task<bool> MeterExists(this DatabaseContext context, string meterId)
        {
            return await context.Meters.AnyAsync(m => m.MeterID == meterId);
        }

        /// <summary>
        /// Add new meter to database
        /// </summary>
        /// <param name="context"></param>
        /// <param name="newMeter"></param>
        /// <returns></returns>
        public static async Task<MeterModel> AddNewMeter(this DatabaseContext context, MeterModel newMeter)
        {
            await context.Meters.AddAsync(newMeter);
            await context.SaveChangesAsync();
            return newMeter;
        }

        /// <summary>
        /// remove a meter from the database 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="meter"></param>
        /// <returns></returns>
        public static async Task<MeterModel> DeleteMeter(this DatabaseContext context, MeterModel meter)
        {
            context.Meters.Remove(meter);
            await context.SaveChangesAsync();
            return meter;
        }

        /// <summary>
        /// returns all meters in database
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<List<MeterModel>> GetAllMeters(this DatabaseContext context)
        {
            return await context.Meters.ToListAsync();
        }

        /// <summary>
        /// try find a meter using its ID
        /// </summary>
        /// <param name="context"></param>
        /// <param name="meterId"></param>
        /// <returns></returns>
        public static async Task<MeterModel> FindMeterByID(this DatabaseContext context, string meterId)
        {
            return await context.Meters.FirstOrDefaultAsync(m => m.MeterID == meterId);
        }

        /// <summary>
        /// update meter properties
        /// </summary>
        /// <param name="context"></param>
        /// <param name="meterUpdate"></param>
        /// <returns></returns>
        public static async Task<MeterModel> UpdateMeter(this DatabaseContext context, MeterModel meterUpdate)
        {
            var meter = await context.Meters.FirstOrDefaultAsync(m => m.MeterID == meterUpdate.MeterID);
            MeterModel meterToUpdate = null;
            if(meter != null)
            {
                meterToUpdate = UpdateMeterProperties(meter,meterUpdate);
                context.Meters.Update(meterToUpdate);
                await context.SaveChangesAsync();
            }
            return meterToUpdate;
        }

        public static async Task<MeterTimeRecords> UpdateMeterTimeRecords(this DatabaseContext context, MeterTimeRecords timeRecordsUpdate)
        {
            var timeRecord = await context.MeterTimeRecords.FirstOrDefaultAsync(m => m.MeterID == timeRecordsUpdate.MeterID);
            MeterTimeRecords newTimeRecords = null;
            if(timeRecord != null)
            {
                newTimeRecords = UpdateMeterTimeProperties(timeRecord,timeRecordsUpdate);
                context.MeterTimeRecords.Update(newTimeRecords);
                await context.SaveChangesAsync();
            }
            return newTimeRecords;
        }

        public static async Task<MeterTariff> GetMeterTariffAsync(this DatabaseContext context,MeterModel Meter)
        {
            return await context.MeterTariffs.FirstOrDefaultAsync(mt => mt.TarrifID == Meter.TarriffID);
        }

        public static async Task<MeterType> GetMeterTypeAsync(this DatabaseContext context,MeterModel Meter)
        {
            return await context.MeterTypes.FirstOrDefaultAsync(mt => mt.ID == Meter.TypeID);
        }

        public static async Task<MeterTimeRecords> GetMeterTimeRecordsAsync(this DatabaseContext context,string meterID)
        {
            return await context.MeterTimeRecords.FirstOrDefaultAsync(mtr => mtr.MeterID == meterID);
        }

        public static async Task<List<string>> FindMetersBySubscriberIDAsync(this DatabaseContext context,SubscriberModel subscriber)
        {
            var meters = await context.SubscriberMeters.Where(sm => sm.SubscriberID == subscriber.Id).ToListAsync();
            var meterIDs = new List<string>();

            if(meters == null)
                return null;
            foreach(var meter in meters)
                meterIDs.Add(meter.MeterID);
            return meterIDs;
        }
        public static async Task<MeterModel> FindMeterBySubscriberIDAsync(this DatabaseContext context,string subscriberID)
        {
            var subMeter = await context.SubscriberMeters.FirstOrDefaultAsync(sm => sm.SubscriberID == subscriberID);
            if(subMeter == null)
            return null;
            return await context.Meters.FirstOrDefaultAsync(m => m.MeterID == subMeter.MeterID);
        }

        public static async Task UpdateMeterState(this DatabaseContext context,MeterModel meter,MeterState state)
        {
            var meterToUpdate = await context.Meters.FirstOrDefaultAsync(m => m.MeterID == meter.MeterID);
            meterToUpdate.MeterState = state.ToString();
            context.Meters.Update(meterToUpdate);
            await context.SaveChangesAsync();
        }

        public static async Task<MeterTimeRecords> AddMeterTimeRecords(this DatabaseContext context,MeterTimeRecords timeRecords)
        {
            await context.MeterTimeRecords.AddAsync(timeRecords);
            await context.SaveChangesAsync();
            return timeRecords;
        }

        public static async Task<bool> MeterTimeRecordsExists(this DatabaseContext context, string meterID)
        {
            return await context.MeterTimeRecords.AnyAsync(m => m.MeterID == meterID);
        }

        public static async Task<MeterLatestReadings> GetMeterLatestReadingsAsync(this DatabaseContext context,MeterModel meter)
        {
            return await context.MeterReadings.FirstOrDefaultAsync(m => m.MeterID == meter.MeterID);
        }

        public static async Task<MeterLatestReadings> GetMeterLatestReadingsAsync(this DatabaseContext context,string meterID)
        {
            return await context.MeterReadings.FirstOrDefaultAsync(m => m.MeterID == meterID);
        }
    }
}