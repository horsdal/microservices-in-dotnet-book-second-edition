namespace LoyaltyProgramServiceTests.Scenarios
{
  using System;
  using System.Net;
  using System.Net.Http;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;
  using LoyaltyProgram;
  using LoyaltyProgram.Users;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.TestHost;
  using Microsoft.Extensions.Hosting;
  using Mocks;
  using Xunit;

  public class RegisterUserAndGetNotification : IDisposable
  {
    private static int mocksPort = 5050;
    private readonly MocksHost serviceMock;
    private readonly IHost loyaltyProgramHost;
    private readonly HttpClient sut;

    public RegisterUserAndGetNotification()
    {
      this.serviceMock = new MocksHost(mocksPort);
      this.loyaltyProgramHost = new HostBuilder()
        .ConfigureWebHost(x => x
          .UseStartup<Startup>()
          .UseTestServer())
        .Start();
      this.sut = this.loyaltyProgramHost.GetTestClient();
    }

    [Fact]
    public async Task Scenario()
    {
      await RegisterNewUser();
      await RunConsumer();
      AssertNotificationWasSent();
    }

    private async Task RegisterNewUser()
    {
      var actual = await this.sut.PostAsync(
        "/users",
        new StringContent(
          JsonSerializer.Serialize(new LoyaltyProgramUser(0, "Chr", 0, new LoyaltyProgramSettings())),
          Encoding.UTF8,
          "application/json"));

      Assert.Equal(HttpStatusCode.Created, actual.StatusCode);
    }

    private Task RunConsumer() =>
      EventConsumer.ConsumeBatch(
        0,
        100,
        $"http://localhost:{mocksPort}/specialoffers",
        $"http://localhost:{mocksPort}"
      );

    private void AssertNotificationWasSent() => Assert.True(NotificationsMock.ReceivedNotification);

    public void Dispose()
    {
      this.serviceMock?.Dispose();
      this.sut?.Dispose();
      this.loyaltyProgramHost?.Dispose();
    }
  }
}