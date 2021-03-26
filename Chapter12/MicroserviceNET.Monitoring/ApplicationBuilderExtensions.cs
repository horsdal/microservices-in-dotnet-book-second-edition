namespace MicroserviceNET.Monitoring
{
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Diagnostics.HealthChecks;

  public static class ApplicationBuilderExtensions
  {
    public static IApplicationBuilder UseKubernetesHealthChecks(this IApplicationBuilder app) =>
      app
        .UseHealthChecks("/health/startup", new HealthCheckOptions {Predicate = x => x.Tags.Contains("startup")})
        .UseHealthChecks("/health/live", new HealthCheckOptions { Predicate = x => x.Tags.Contains("liveness")});
  }
}