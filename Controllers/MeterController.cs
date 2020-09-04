using System.Net.Mime;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices.Provisioning.Client;
using Microsoft.Azure.Devices.Provisioning.Client.Transport;
using Microsoft.Azure.Devices.Shared;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Microsoft.EntityFrameworkCore;

namespace LagosSmartMeter
{
    [ApiController]
    [Route("meter")]
    public class AuthenticateMeter : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly string DPSGlobalEndpoint;
        private readonly string ScopeID;
        private readonly DatabaseContext _context;

        public AuthenticateMeter(IConfiguration configuration,DatabaseContext context)
        {
            this.configuration = configuration;

            // TODO :: CONFIGURE TO FETCH FROM DATABASE
            DPSGlobalEndpoint = configuration.GetValue<string>("DPS:GlobalEndpoint");
            ScopeID = configuration.GetValue<string>("DPS:ScopeID");
            _context = context;
        }

        #region Helper Methods

        private static ResponseApiModel ErrorResponse(int errorCode)
        {
            return new ResponseApiModel{ ErrorMessage = errorCode.ToString()};
        }

        private static void Log(string message)
        {
            //System.IO.File.AppendAllText("/home/exploit90/Desktop/logfile.txt",message);
            Console.Write("log >> ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static void LogError(int errorCode)
        {
            System.IO.File.AppendAllText("/home/exploit90/Desktop/logfile.txt",$"Error returned {errorCode.ToString()}");
            Console.Write("error >>");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error returned {errorCode}");
            Console.ResetColor();
        }
        #endregion

        [HttpPost][Route("authentication")]
        public async Task<ActionResult<ResponseApiModel<AuthenticateMeterResponseApiModel>>> Authenticate(
            [FromBody]AuthenticateMeterRequestApiModel meterCredentials)
        {
            Log("Starting Request");
            // fail if model is invalid
            if(meterCredentials == null)
            {
                LogError(ErrorCodes.InvalidRequestBody);
                return BadRequest(ErrorResponse(ErrorCodes.InvalidRequestBody));
            }

            // fail if model content is invalid
            if(meterCredentials.x509Certificate == null ||
                string.IsNullOrWhiteSpace(meterCredentials.Password))
            {
                LogError(ErrorCodes.InvalidCertificate + 10);
                return BadRequest(ErrorResponse(ErrorCodes.InvalidRequestBody));
            }

            Log("Request Model Validated!!");
            // create x509 certificate to authenticate meter with
            X509Certificate2 x509Certificate;
            try{
                x509Certificate = new X509Certificate2(meterCredentials.x509Certificate,meterCredentials.Password);
            }
            // fail if exception is thrown while creating certificate           
            catch{
                LogError(ErrorCodes.InvalidCertificate);
                return BadRequest(ErrorResponse(ErrorCodes.InvalidCertificate));
            }
            Log("Request Certificate file Validated!!");

            // create device provisioning client
            DeviceRegistrationResult result = null;
            using(var securityProvider = new SecurityProviderX509Certificate(x509Certificate))
            {
                using(var transportHandler = new ProvisioningTransportHandlerAmqp(TransportFallbackType.TcpOnly))
                {
                    try{
                        Log("Attempting device provisioning");
                        var _client = ProvisioningDeviceClient.Create(DPSGlobalEndpoint,ScopeID,securityProvider,transportHandler);
                        result = await _client.RegisterAsync();
                    }
                    catch{
                        LogError(600);
                    }

                    // fail if device registration attempt failed
                    if(result == null)
                    {
                        LogError(ErrorCodes.RegistrationAttemptFailure);
                        return Unauthorized(ErrorResponse(ErrorCodes.RegistrationAttemptFailure));
                    }

                    // return fail if device provisioning was unsuccessful
                    if(result.Status == ProvisioningRegistrationStatusType.Failed)
                    {
                        LogError(ErrorCodes.RegistrationAttemptFailure);
                        Log("Device registration failed");
                        return Unauthorized(ErrorResponse(ErrorCodes.RegistrationAttemptFailure));
                    }

                    Log("Device Provisioned Successfully!!");
                    Log("Updating Database...");
                    // register device in database if it does not exist
                    var tarrif = await _context.MeterTariffs.FirstOrDefaultAsync(mt => mt.Name == "Commercial usage 12500");
                    var meterType = await _context.MeterTypes.FirstOrDefaultAsync(mt => mt.Name == "GSI");
                    if(!(await _context.MeterExists(result.DeviceId)))
                        await _context.AddNewMeter(new MeterModel {
                            MeterID = result.DeviceId,
                            MeterState = "unknown",
                            ProvisionState = result.Status.ToString().ToLower(),
                            TarriffID = tarrif.TarrifID,
                            TypeID = meterType.ID
                        });
                    else
                    { 
                        var meterUpdate = await _context.FindMeterByID(result.DeviceId);
                        meterUpdate.ProvisionState = result.Status.ToString().ToLower();
                        await _context.UpdateMeter(meterUpdate);
                    }

                    // add device time records
                    if(!await _context.MeterTimeRecordsExists(result.DeviceId))
                    {
                        await _context.AddMeterTimeRecords(new MeterTimeRecords {
                            MeterID = result.DeviceId,
                            LastActivation = DateTime.MinValue,
                            InitialActivation = DateTime.UtcNow,
                            LastStartUp = DateTime.UtcNow,
                            LastUpdate = DateTime.UtcNow,
                            LastDeactivation = DateTime.MinValue,
                            LastShutDown = DateTime.MinValue
                        });
                    }
                    else
                    {
                        var meterTimeUpdate = await _context.GetMeterTimeRecordsAsync(result.DeviceId);
                        meterTimeUpdate.LastUpdate = DateTime.UtcNow;
                        meterTimeUpdate.LastStartUp = DateTime.UtcNow;
                        await _context.UpdateMeterTimeRecords(meterTimeUpdate);
                    }
                    // fetch last meter readings
                    var lastReadings = await _context.GetMeterLatestReadingsAsync(result.DeviceId);
                    Log("Database Updated!!");
                    // return device registration result 
                    ResponseApiModel response = new ResponseApiModel<AuthenticateMeterResponseApiModel>{
                        Body = new AuthenticateMeterResponseApiModel{
                                DeviceID = result.DeviceId,
                                AssignedIoTHub = result.AssignedHub,
                                MeterLastEnergyReadings = new MeterLastEnergyReadings{
                                    AvailableEnergy = lastReadings == null ? 0 : lastReadings.AvailableEnergy,
                                    UsedEnergyInHours  = lastReadings == null ? 0 : lastReadings.EnergyUsedInHours,
                                    UsedEnergyInSeconds = lastReadings == null ? 0 : lastReadings.EnergyUsedInSeconds
                                }
                        }};
                    Log("Request Completed!!");
                    return Ok(response);    
                }  
            }                   
        }

        [HttpPost][Route("topup")]
        public async Task<ActionResult<ResponseApiModel<TopUpMeterResponseApiModel>>> TopUpMeter(
            [FromBody]TopUpMeterRequestApiModel topUp)
        {
                // validate request model
                if(topUp == null)
                    return BadRequest(ErrorResponse(ErrorCodes.TopUpFailed));
                
                // check for empty request parameters
                if(string.IsNullOrWhiteSpace(topUp.MeterID) || 
                    string.IsNullOrWhiteSpace(topUp.Token))
                    return BadRequest(ErrorResponse(ErrorCodes.TopUpFailed));
                
                // try find the meter
                var meter = await _context.FindMeterByID(topUp.MeterID);

                // fail if meter is not found
                if(meter == null)
                    return BadRequest(ErrorResponse(ErrorCodes.TopUpFailed));
                
                // try find token
                var token = await _context.FindUnusedMeterToken(topUp.Token);
                
                // fail if token is not found
                if(token == null)
                    return BadRequest(ErrorResponse(ErrorCodes.TopUpFailed));
                
                // set token to used
                await _context.UsedMeterToken(token);
                
                // return token value
                return Ok(new ResponseApiModel<TopUpMeterResponseApiModel>{
                    Body = new TopUpMeterResponseApiModel{
                        Value = token.Value
                    }
                });
        }
    }
}