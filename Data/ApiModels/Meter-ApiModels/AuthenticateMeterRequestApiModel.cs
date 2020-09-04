using System;
using System.ComponentModel.DataAnnotations;

namespace LagosSmartMeter
{
    public class AuthenticateMeterRequestApiModel
    {   
        [Required]
        /// <summary>
        /// unique x509 certificate embedded in device HSM
        /// </summary>
        /// <value></value>
        public Byte[] x509Certificate { get; set; } 

        [Required]
        /// <summary>
        /// password used in signing device 
        /// </summary>
        /// <value></value>
        public string Password { get; set; }
    }
}