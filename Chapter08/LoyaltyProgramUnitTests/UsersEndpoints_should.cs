namespace LoyaltyProgramUnitTests
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
  using Xunit;

  public class UsersEndpoints_should : IDisposable
  {
    private readonly IHost host;
    private readonly HttpClient sut;

    public UsersEndpoints_should()
    {
      this.host = new HostBuilder()
        .ConfigureWebHost(x => x
          .UseStartup<Startup>()
          .UseTestServer())
        .Start();
      this.sut = this.host.GetTestClient();
    }

    [Fact]
    public async Task respond_not_fount_when_queried_for_unregistered_user()
    {
      var actual = await this.sut.GetAsync("/users/1000");
      Assert.Equal(HttpStatusCode.NotFound, actual.StatusCode);
    }

    [Fact]
    public async Task allow_to_register_new_user()
    {
      var expected = new LoyaltyProgramUser(0, "Christian", 0, new LoyaltyProgramSettings());

      var registrationResponse = await this.sut.PostAsync("/users",
        new StringContent(JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json"));
      var newUser =
        await JsonSerializer.DeserializeAsync<LoyaltyProgramUser>(
          await registrationResponse.Content.ReadAsStreamAsync(),
          new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

      var actual = await this.sut.GetAsync($"/users/{newUser?.Id}");
      var actualUser = JsonSerializer.Deserialize<LoyaltyProgramUser>(await actual.Content.ReadAsStringAsync(),
        new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

      Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
      Assert.Equal(expected.Name, actualUser?.Name);
    }

    [Fact]
    public async Task allow_modifying_users()
    {
      var expected = "jane";
      var user = new LoyaltyProgramUser(0, "Christian", 0, new LoyaltyProgramSettings());
      var registrationResponse = await this.sut.PostAsync(
        "/users",
        new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json"));
      var newUser = await
        JsonSerializer.DeserializeAsync<LoyaltyProgramUser>(
          await registrationResponse.Content.ReadAsStreamAsync(),
          new JsonSerializerOptions {PropertyNameCaseInsensitive = true})!;

      var updatedUser = newUser! with {Name = expected};
      var actual = await this.sut.PutAsync($"/users/{newUser.Id}",
        new StringContent(JsonSerializer.Serialize(updatedUser), Encoding.UTF8, "application/json"));
      var actualUser = await JsonSerializer.DeserializeAsync<LoyaltyProgramUser>(
        await actual.Content.ReadAsStreamAsync(),
        new JsonSerializerOptions {PropertyNameCaseInsensitive = true});

      Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
      Assert.Equal(expected, actualUser?.Name);
    }

    public void Dispose()
    {
      this.host.Dispose();
      this.sut.Dispose();
    }
  }
}