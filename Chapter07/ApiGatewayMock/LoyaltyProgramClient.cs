namespace ApiGatewayMock
{
  using System.Net.Http;
  using System.Text;
  using System.Threading.Tasks;
  using System.Text.Json;

  public class LoyaltyProgramClient
  {
    private readonly HttpClient httpClient;

    public LoyaltyProgramClient(HttpClient httpClient) => this.httpClient = httpClient;

    public async Task<HttpResponseMessage> RegisterUser(string name)
    {
      var user = new {name, Settings = new { }};
      return await this.httpClient.PostAsync("/users/",
        CreateBody(user));
    }

    private static StringContent CreateBody(object user) =>
      new StringContent(
        JsonSerializer.Serialize(user),
        Encoding.UTF8,
        "application/json");

    public async Task<HttpResponseMessage> QueryUser(string arg) => await this.httpClient.GetAsync($"/users/{int.Parse(arg)}");

    public async Task<HttpResponseMessage> UpdateUser(LoyaltyProgramUser user) =>
      await this.httpClient.PutAsync($"/users/{user.Id}", CreateBody(user));
  }
}