using System;
using Microsoft.AspNetCore.Identity;

namespace LagosSmartMeter
{
    public class TemporaryData
    {
        private readonly DatabaseContext context;
        private readonly UserManager<SubscriberModel> userManager;
        public TemporaryData(
            UserManager<SubscriberModel> userManager,
        DatabaseContext context)
        {
            this.context = context;
            this.userManager = userManager;
        }

    public void Add()
    {
        // add test subscriber
        var subscriber = new SubscriberModel
        {
            FullName = "Adebola Ciromanna Chukwuma",
            UserName = "ciromannachukwuma@resolve.com",
            Email = "ciromannachukwuma@resolve.com",
            Gender = "male",
            Birthday = new DateTime(1984, 09, 09),
        };
        var result = userManager.CreateAsync(subscriber, "qwerty12").GetAwaiter().GetResult();
        if (result.Succeeded)
        {
            var user = userManager.FindByEmailAsync(subscriber.Email).GetAwaiter().GetResult();
            // subscriber address
            var address = new SubscriberAddress{
                SubscriberID = user.Id,
                city = "Ikeja",
                Street = "Ajimola",
                BuildingNumber = "32",
                Phone = "+2348123456789"
            };

            // subscriber meter
            var testMeter = new SubscriberMetersModel{
                SubscriberID = user.Id,
                MeterID = "0173826387"
            };
            context.SubscriberAddresses.Add(address);
            
            // meter tariff
            var tariff = new MeterTariff{
                TarrifID = "tarrif-" + Guid.NewGuid().ToString(),
                AccessRatePeriod = 7,
                HasAccessRate = true,
                AccessRatePrice = 0,
                KwhPrice = 12500,
                Name = "Commercial usage 12500"
            };

            // meter type
            var meterType = new MeterType{
                Name = "GSI",
                Description = "Meter used during Test",
                ID = "metertype-" + Guid.NewGuid().ToString()
            };
            user.PhoneNumber = address.Phone;

            context.SubscriberAddresses.Add(address);
            context.SubscriberMeters.Add(testMeter);
            context.MeterTariffs.Add(tariff);
            context.MeterTypes.Add(meterType);
            context.SaveChanges();
        }
    }
}}