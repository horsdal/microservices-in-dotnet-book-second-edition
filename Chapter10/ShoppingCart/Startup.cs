namespace ShoppingCart
{
  using System;
  using System.Linq;
  using EventFeed;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Diagnostics.HealthChecks;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Diagnostics.HealthChecks;
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
              && t != typeof(EsEventStore)
              && t.GetMethods().All(m => m.Name != "<Clone>$")))
          .AsImplementedInterfaces());
      services.AddHttpClient<IProductCatalogClient, ProductCatalogClient>()
        .AddTransientHttpErrorPolicy(p =>
          p.WaitAndRetryAsync(
            3,
            attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
            (ex, _) => Console.WriteLine(ex.ToString())));
      services.AddHealthChecks()
        .AddCheck<DbHealthCheck>(nameof(DbHealthCheck), tags: new []{ "startup"})
        .AddCheck("LivenessHealthCheck", () => HealthCheckResult.Healthy(), tags: new []{ "liveness"});
      services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
      app.UseRouting();
      app.UseHealthChecks("/health/startup", new HealthCheckOptions {Predicate = x => x.Tags.Contains("startup")});
      app.UseHealthChecks("/health/live", new HealthCheckOptions { Predicate = x => x.Tags.Contains("liveness")});
      app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
  }
}