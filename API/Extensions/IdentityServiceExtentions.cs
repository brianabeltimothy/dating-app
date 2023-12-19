using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions
{
    public static class IdentityServiceExtentions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => 
                {
                    //Configure the JwtBearer authentication options, set up rules for checking if a JWT is valid and can be trusted.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //Specify that the validation should include checking the issuer signing key, to make sure the token's signature is correct, as long as its jwt, the server accept
                        ValidateIssuerSigningKey = true,

                        // Set the issuer signing key to a symmetric key based on the TokenKey stored in the configuration, provide a new signature that is use to check the incoming signature
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])),

                        // Disable issuer validation (whether the issuer is trusted) for simplicity.
                        ValidateIssuer = false,

                        // Disable audience validation (whether the audience is trusted) for simplicity.
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}