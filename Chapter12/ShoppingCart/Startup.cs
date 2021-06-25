namespace ShoppingCart
{
  using System;
  using System.Linq;
  using EventFeed;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.Extensions.DependencyInjection;
  using Polly;

  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services
        .Scan(selector => selector.FromAssemblyOf<Startup>()
          .AddClasses(c =>
            c.Where(t =>
              t != typeof(ProductCatalogClient)
              && t != typeof(SqlEventStore)
              && t.GetMethods().All(m => m.Name != "<Clone>$")))
          .AsImplementedInterfaces());
      services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
        .AddTransientHttpErrorPolicy(p =>
          p.WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
            (ex, _) => Console.WriteLine(ex.ToString())));
      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseRouting();
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}