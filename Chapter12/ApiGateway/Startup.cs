namespace ApiGateway
{
  using System;
  using MicroserviceNET.Monitoring;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc.Razor;
  using Microsoft.Extensions.DependencyInjection;
  using Polly;

  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddBasicHealthChecks();
      services.AddControllersWithViews();
      services.Configure<RazorViewEngineOptions>(x =>
        x.ViewLocationFormats.Add("{1}/{0}.cshtml"));
      services.AddHttpClient("ProductCatalogClient", client => client.BaseAddress = new Uri("https://localhost:5001"))
        .AddTransientHttpErrorPolicy(p =>
          p.WaitAndRetryAsync(3, attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt))));
      services.AddHttpClient("ShoppingCartClient", client => client.BaseAddress = new Uri("https://localhost:5201"));

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      app.UseRouting();
      app.UseKubernetesHealthChecks();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}