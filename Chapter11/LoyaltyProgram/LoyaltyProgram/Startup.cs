namespace LoyaltyProgram
{
  using System.IdentityModel.Tokens.Jwt;
  using Microsoft.AspNetCore.Authentication.JwtBearer;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.IdentityModel.Tokens;

  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.TokenValidationParameters = new TokenValidationParameters
        { // DO NOT USE THIS CONFIGURATION IN PRODUCTION
          ValidateIssuer = false,
          ValidateAudience = false,
          ValidateIssuerSigningKey = false,
          SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
        });
      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseHttpsRedirection();
      app.UseRouting();
      app.UseAuthorization();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}