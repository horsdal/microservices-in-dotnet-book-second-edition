using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiGatewayMock;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using static System.Console;
using static System.Environment;

var exponentialRetryPolicy =
  Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrTransientHttpStatusCode()
    .WaitAndRetryAsync(
      3,
      attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)));

var circuitBreakerPolicy =
  Policy<HttpResponseMessage>
    .Handle<HttpRequestException>()
    .OrTransientHttpStatusCode()
    .CircuitBreakerAsync(5, TimeSpan.FromMinutes(1));

var host = args.Length > 0 ? args[0] : "https://localhost:5001";

var serviceProvider =
  new ServiceCollection()
    .AddHttpClient<LoyaltyProgramClient>()
    .AddPolicyHandler(request =>
      request.Method == HttpMethod.Get
        ? circuitBreakerPolicy
        : exponentialRetryPolicy)
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(host))
    .Services
    .BuildServiceProvider();

var client = serviceProvider.GetService<LoyaltyProgramClient>()!;

var processCommand =
  new Dictionary<char, (string description, Func<string, Task<(bool, HttpResponseMessage)>> handler)>
  {
    {
      'r',
      ("r <user name> - to register a user with name <user name> with the Loyalty Program Microservice.",
        async c => (true, await client.RegisterUser(c.Substring(1))))
    },
    {
      'q',
      ("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.",
        async c => (true, await client.QueryUser(c.Substring(1))))
    },
    {
      'u',
      ("u <userid> <interests> - to update a user with new interests",
        HandleUpdateInterestsCommand)
    },
    {
      'x',
      ("x - to exit",
        _ => Task.FromResult((false, new HttpResponseMessage(0))))
    },
  };

WriteLine("Welcome to the API Gateway Mock.");
var cont = true;
while (cont)
{
  WriteLine();
  WriteLine();
  WriteLine("********************");
  WriteLine("Choose one of:");
  foreach (var c in processCommand.Values)
    WriteLine(c.description);
  WriteLine("********************");
  var cmd = ReadLine() ?? string.Empty;
  if (processCommand.TryGetValue(cmd[0], out var command))
  {
    var (@continue, response) = await command.handler(cmd);
    await PrettyPrint(response);
    cont = @continue;
  }
}

async Task<(bool, HttpResponseMessage)> HandleUpdateInterestsCommand(string cmd)
{
  var response = await client.QueryUser(cmd.Split(' ').Skip(1).First());
  if (!response.IsSuccessStatusCode)
    return (true, response);

  var user = JsonSerializer.Deserialize<LoyaltyProgramUser>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
  if (user is null)
    return (true, response);
  var newInterests = cmd[cmd.IndexOf(' ', 2)..].Split(',').Select(i => i.Trim());
  var res = await client.UpdateUser(
    user with
    {
      Settings = user.Settings with
      {
        Interests = user.Settings.Interests.Concat(newInterests).ToArray()
      }
    });
  return (true, res);}

static async Task PrettyPrint(HttpResponseMessage response)
{
  if (response.StatusCode == 0) return;
  WriteLine("********** Response **********");
  WriteLine($"status code: {response.StatusCode}");
  WriteLine("Headers: " + response.Headers.Aggregate("",
    (acc, h) => $"{acc}{NewLine}\t{h.Key}: {h.Value.Aggregate((hAcc, hVal) => $"{hAcc}{hVal}, ")}") ?? "");
  if (response.Content.Headers.ContentLength > 0)
    WriteLine(@$"Body:{NewLine}{
      JsonSerializer.Serialize(await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync()), new JsonSerializerOptions {WriteIndented = true})
    }");
}

public record LoyaltyProgramUser(int Id, string Name, int LoyaltyPoints, LoyaltyProgramSettings Settings);

public record LoyaltyProgramSettings()
{
  public LoyaltyProgramSettings(string[] interests) : this()
  {
    this.Interests = interests;
  }

  public string[] Interests { get; init; } = Array.Empty<string>();
}