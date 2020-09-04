using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Configuration;

namespace LagosSmartMeter
{
    [ApiController]
    [Route("api/subscriber")]
    public class SubscriberController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly IConfiguration _configuration;
        private readonly DeviceService _deviceService;
        private readonly UserManager<SubscriberModel> _subscriberUserManager;
        private readonly SignInManager<SubscriberModel> _subscriberSignInManager;

        public SubscriberController(
            DatabaseContext context,
            IConfiguration configuration,
            DeviceService deviceService,
            UserManager<SubscriberModel> subscriberUserManager,
            SignInManager<SubscriberModel> subscriberSignInManager)
        {
            _context = context;
            _configuration = configuration;
            _deviceService = deviceService;
            _subscriberUserManager = subscriberUserManager;
            _subscriberSignInManager = subscriberSignInManager;
        }

        #region Helper-Methods
        private static ResponseApiModel ErrorResponse(string errorMessage)
        {
            return new ResponseApiModel{ErrorMessage  = errorMessage};
        }

        private static void Log(string message)
        {
            //System.IO.File.AppendAllText("/home/exploit90/Desktop/logfile.txt",message);
            Console.Write("log >> ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        #endregion

        [HttpPost()][Route(SubscriberApiRoutes.Login)]
        public async Task<ActionResult<SubscriberLoginResponse>> Login(
            [FromBody]SubscriberLoginCredentials loginCredentials
        ){
            // error messages
            var unAuthorized = "Invalid Login Credentials";
            var errorResponse = ErrorResponse(unAuthorized);

            // check if model is invalid
            if(loginCredentials == null)
                return Unauthorized(errorResponse);
            
            // check if any login parameter is empty
            if(string.IsNullOrWhiteSpace(loginCredentials.Email) || 
                string.IsNullOrWhiteSpace(loginCredentials.Password))
                return Unauthorized(errorResponse);
            
            // try find user from database
            var user = await _subscriberUserManager.FindByEmailAsync(loginCredentials.Email);

            // fail if user does not exists
            if(user ==  null)
                return Unauthorized(errorResponse);
            
            // check user password if user exists
            var result = await _subscriberUserManager.CheckPasswordAsync(user,loginCredentials.Password);

            // fail if password is incorrect
            if(!result)
                return Unauthorized(errorResponse);
            
            // fetch user address from database
            var address = await _context.GetAllSubscriberAddresses(user);
            //fetch user meters
            var userMeter = await _context.FindMeterBySubscriberIDAsync(user.Id);
            // login successful
            return Ok(new SubscriberLoginResponse{
                    Name = user.FullName,
                    Email = user.Email,
                    Birthday = user.Birthday,
                    Gender = user.Gender,
                    Address = $"No {address.BuildingNumber},{address.Street} street,{address.city},{address.Phone}", 
                    // Address = new Address{
                    //     city = userAddress.city,
                    //     BuildingNumber = userAddress.BuildingNumber,
                    //     Street = userAddress.Street,
                    //     Phone = userAddress.Phone
                    // },
                    MeterID = userMeter.MeterID,
                    Token = user.GenerateJwtToken(_configuration)
                });
        }

        // [HttpGet]
        // public async Task<ActionResult<ResponseApiModel>> GetTestSubscriber()
        // {
        //     var subscriber = await _context.GetSubscriberAsync();
        //     var subscriberAddress = new List<SubscriberAddressDetails>();
        //     var meter = await _context.FindMeterBySubscriberIDAsync(subscriber.SubscriberID);
        //     var meterTariff = await _context.GetMeterTariffAsync(meter);
        //     var meterType = await _context.GetMeterTypeAsync(meter);
        //     var meterTimeRecords = await _context.GetMeterTimeRecordsAsync(meter.MeterID);
        //     foreach(var add in await _context.GetAllSubscriberAddresses(subscriber))
        //         subscriberAddress.Add(new SubscriberAddressDetails{
        //             city = add.city,
        //             Street = add.Street,
        //             Phone = add.Phone,
        //             IsPrimary = add.IsPrimary,
        //             BuildingNumber = add.BuildingNumber
        //         });
        //     var subscriberMeter = new SubscriberMeterDetails{
        //         MeterID = meter.MeterID,
        //         MeterTypeDescription = meterType.Description,
        //         MeterType = meterType.Name,
        //         MeterState = meter.MeterState,
        //         InitialActivation = meterTimeRecords != null ? meterTimeRecords.InitialActivation : DateTime.MinValue,
        //         LastActivation = meterTimeRecords != null ? meterTimeRecords.LastActivation : DateTime.MinValue,
        //         LastDeactivation = meterTimeRecords != null ? meterTimeRecords.LastDeactivation : DateTime.MinValue,
        //         LastShutDown = meterTimeRecords != null ? meterTimeRecords.LastShutDown : DateTime.MinValue,
        //         LastStartUp = meterTimeRecords != null ? meterTimeRecords.LastStartUp : DateTime.MinValue,
        //         LastUpdate = meterTimeRecords != null ? meterTimeRecords.LastUpdate : DateTime.MinValue,
        //     };

        //     var meterTariffDetails = new MeterTariffDetails{
        //         TariffName = meterTariff.Name,
        //         KwhPrice = meterTariff.KwhPrice,
        //         HasAccessRate = meterTariff.HasAccessRate,
        //         AccessRatePeriod = meterTariff.AccessRatePeriod,
        //         AccessRatePrice = meterTariff.AccessRatePrice
        //     };

        //     var userDetails = new SubscriberDetails{
        //         FullName = subscriber.FullName,
        //         EmailAddress = subscriber.EmailAddress,
        //         Addresses = subscriberAddress,
        //         Meter = subscriberMeter,
        //         Tariff = meterTariffDetails
        //     };
        //     return Ok(userDetails);
        // }

        // [HttpGet] [Route("activate-meter")]
        // public async Task<ActionResult<ResponseApiModel>> ActivateDevice()
        // {
        //     Log("Request to activate meter...");
        //     var subscriber = await _context.GetSubscriberAsync();
        //     var meter = await _context.FindMeterBySubscriberIDAsync(subscriber.SubscriberID);
        //     var result = await _deviceService.ActivateDevice(_configuration,meter.MeterID);
        //     if(!result)
        //     {
        //         Log("Failed to activate Meter");
        //         return BadRequest();
        //     }
        //     await _context.UpdateMeterState(meter,MeterState.active);
        //     var meterTimeRecords = await _context.GetMeterTimeRecordsAsync(meter.MeterID);
        //     meterTimeRecords.LastActivation = DateTime.UtcNow;
        //     await _context.UpdateMeterTimeRecords(meterTimeRecords);
        //     Log("Meter Activated!!");
        //     return Ok();
        // }

        // [HttpGet] [Route("deactivate-meter")]
        // public async Task<ActionResult<ResponseApiModel>> DeactivateDevice()
        // {
        //     Log("Request to deactivate meter...");
        //     var subscriber = await _context.GetSubscriberAsync();
        //     var meter = await _context.FindMeterBySubscriberIDAsync(subscriber.SubscriberID);
        //     var result = await _deviceService.DeactivateDevice(_configuration,meter.MeterID);
        //     if(!result)
        //     {
        //         Log("Failed to deactivate meter!!");
        //         return BadRequest();
        //     }
        //     var meterTimeRecords = await _context.GetMeterTimeRecordsAsync(meter.MeterID);
        //     meterTimeRecords.LastDeactivation = DateTime.UtcNow;
        //     await _context.UpdateMeterTimeRecords(meterTimeRecords);
        //     await _context.UpdateMeterState(meter,MeterState.standby);
        //     Log("Meter Deactivated!!");
        //     return Ok();
        // }
    }
}