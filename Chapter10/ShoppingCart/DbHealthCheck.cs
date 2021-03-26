namespace ShoppingCart
{
  using System.Data.SqlClient;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using Dapper;
  using Microsoft.Extensions.Diagnostics.HealthChecks;
  using Microsoft.Extensions.Logging;

  public class DbHealthCheck : IHealthCheck
  {
    private readonly HttpClient httpClient;
    private readonly ILogger logger;

    public DbHealthCheck(ILoggerFactory logger)
    {
      this.httpClient = new HttpClient();
      this.logger = logger.CreateLogger<DbHealthCheck>();
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
    {
      await using var conn = new SqlConnection("Data Source=localhost;Initial Catalog=master;User Id=SA; Password=yourStrong(!)Password");
      var result = await conn.QuerySingleAsync<int>("SELECT 1");
      return result == 1
        ? HealthCheckResult.Healthy()
        : HealthCheckResult.Degraded();
    }
  }
}