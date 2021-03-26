namespace LoyaltyProgramUnitTests
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.TestHost;
  using Microsoft.Extensions.Hosting;
  using Xunit;

  public class TestController_should : IDisposable
  {
    private readonly IHost host;
    private readonly HttpClient sut;

    public class TestController : ControllerBase
    {
      [HttpGet("/")]
      public OkResult Get() => Ok();
    }

    public TestController_should()
    {
      this.host = new HostBuilder()
        .ConfigureWebHost(host =>
          host
            .ConfigureServices(x => x.AddControllersByType(typeof(TestController)))
            .Configure(x => x.UseRouting().UseEndpoints(opt => opt.MapControllers()))
            .UseTestServer())
        .Start();
      this.sut = this.host.GetTestClient();
    }

    [Fact]
    public async Task respond_ok_to_request_to_root()
    {
      var actual = await this.sut.GetAsync("/");
      Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
    }

    public void Dispose()
    {
      this.host?.Dispose();
      this.sut?.Dispose();
    }
  }
}