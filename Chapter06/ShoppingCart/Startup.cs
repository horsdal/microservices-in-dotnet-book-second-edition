namespace ShoppingCart
{
  using System;
  using EventFeed;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.Extensions.DependencyInjection;
  using Polly;

  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.Scan(selector => selector.FromAssemblyOf<Startup>().AddClasses(c => c.Where(t => t != typeof(ProductCatalogClient) && t != typeof(EsEventStore))).AsImplementedInterfaces());
      services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
        .AddTransientHttpErrorPolicy(p =>
          p.WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
            (ex, _) => Console.WriteLine(ex.ToString())));
      services.AddControllers().AddXmlSerializerFormatters();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseRouting();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}