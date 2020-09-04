using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LagosSmartMeter
{
    public static class JwtTokenExtentionMethods
    {
        /// <summary>
        /// Generates a Jwt bearer token containing the users username
        /// </summary>
        /// <param name="user">The users details</param>
        /// <returns></returns>
        public static string GenerateJwtToken(this SubscriberModel user, IConfiguration Configuration)
        {
            // Set our tokens claims
            var claims = new[]
            {
                // Unique ID for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

                // The username using the Identity name so it fills out the HttpContext.User.Identity.Name value
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),

                // Add user Id so that UserManager.GetUserAsync can find the user based on Id
                new Claim(ClaimTypes.NameIdentifier, user.Id),

            };

            // Create the credentials used to generate the token
            var credentials = new SigningCredentials(
                // Get the secret key from configuration
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:signingkey"])),
                // Use HS256 algorithm
                SecurityAlgorithms.HmacSha256);

            // Generate the Jwt Tokenb
            var token = new JwtSecurityToken(
                issuer: Configuration["jwt:issuer"],
                audience: Configuration["jwt:audience"],
                claims: claims,
                signingCredentials: credentials,
                // Expire if not used for 3 months
                expires: DateTime.Now.AddMonths(3)
                );

            // Return the generated token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // /// <summary>
        // /// Generates a Jwt bearer token containing the users username
        // /// </summary>
        // /// <param name="user">The users details</param>
        // /// <returns></returns>
        // public static string GenerateStakeHolderJwtToken(this StakeHolder user, IConfiguration Configuration)
        // {
        //     // Set our tokens claims
        //     var claims = new[]
        //     {
        //         // Unique ID for this token
        //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

        //         // The username using the Identity name so it fills out the HttpContext.User.Identity.Name value
        //         new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),

        //         // Add user Id so that UserManager.GetUserAsync can find the user based on Id
        //         new Claim(ClaimTypes.NameIdentifier, user.Id),

        //     };

        //     // Create the credentials used to generate the token
        //     var credentials = new SigningCredentials(
        //         // Get the secret key from configuration
        //         new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:signingkey"])),
        //         // Use HS256 algorithm
        //         SecurityAlgorithms.HmacSha256);

        //     // Generate the Jwt Token
        //     var token = new JwtSecurityToken(
        //         issuer: Configuration["jwt:issuer"],
        //         audience: Configuration["jwt:audience"],
        //         claims: claims,
        //         signingCredentials: credentials,
        //         // Expire if not used for 3 months
        //         expires: DateTime.Now.AddMonths(3)
        //         );

        //     // Return the generated token
        //     return new JwtSecurityTokenHandler().WriteToken(token);
        // }
    }
}
