using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
namespace LagosSmartMeter
{
    /// <summary>
    /// authorization token attribute for json web token
    /// </summary>
    public class AuthorizationTokenAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Default Constructor
        /// </summary>
        public AuthorizationTokenAttribute()
        {
            // Json Web token Validation Scheme
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
