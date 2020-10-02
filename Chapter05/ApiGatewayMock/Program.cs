namespace ApiGatewayMock
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net.Http;
  using System.Threading.Tasks;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using static System.Console;
  using static System.Environment;

  internal class Program
  {
    private LoyaltyProgramClient client;
    private readonly Dictionary<char, (string description, Func<string, Task<(bool, HttpResponseMessage)>> handler)> processCommand;

    public static async Task Main(string[] args) => await new Program(args).Run();

    private Program(string[] args)
    {
      var host = args.Length > 0 ? args[0] : "https://localhost:5001";
      this.client = new LoyaltyProgramClient(new HttpClient { BaseAddress = new Uri(host) });
      this.processCommand =
        new Dictionary<char, (string description, Func<string, Task<(bool, HttpResponseMessage)>> handler)>
        {
          {
            'r',
            ("r <user name> - to register a user with name <user name> with the Loyalty Program Microservice.",
              async c => (true, await this.client.RegisterUser(c.Substring(1))))
          },
          {
            'q',
            ("q <userid> - to query the Loyalty Program Microservice for a user with id <userid>.",
              async c => (true, await this.client.QueryUser(c.Substring(1))))
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
    }

    private async Task<(bool, HttpResponseMessage)> HandleUpdateInterestsCommand(string cmd)
    {
        var response = await this.client.QueryUser(cmd.Split(' ').Skip(1).First());
        if (!response.IsSuccessStatusCode) 
          return (true, response);
        
        var user = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(),
          new {name = "", id = 0, loyaltyPoints = 0, settings = new {interests = new string[0]}});
        var newInterests = cmd.Substring(cmd.IndexOf(' ', 2)).Split(',').Select(i => i.Trim());
        var res = await this.client.UpdateUser(new
        {
          user.name,
          user.id,
          user.loyaltyPoints,
          settings = new {interests = user.settings.interests.Concat(newInterests).ToArray()}
        });
        return (true, res);

    }

    private async Task Run()
    {
      WriteLine("Welcome to the API Gateway Mock.");

      var cont = true;
      while (cont)
      {
        WriteLine();
        WriteLine();
        WriteLine("********************");
        WriteLine("Choose one of:");
        foreach (var c in this.processCommand.Values)
          WriteLine(c.description);
        WriteLine("********************");
        var cmd = ReadLine();
        if (this.processCommand.TryGetValue(cmd[0], out var command))
        {
          var (@continue, response) = await command.handler(cmd);
          await PrettyPrint(response);
          cont = @continue;
        }
      }
    }

    private static async Task PrettyPrint(HttpResponseMessage response)
    {
      if (response.StatusCode == 0) return;
      WriteLine("********** Response **********");
      WriteLine($"status code: {response.StatusCode}");
      WriteLine("Headers: " + response.Headers.Aggregate("",
        (acc, h) => $"{acc}{NewLine}\t{h.Key}: {h.Value.Aggregate((hAcc, hVal)=> $"{hAcc}{hVal}, ")}") ?? "");
      WriteLine($"Body:{NewLine}{JToken.Parse(await response.Content.ReadAsStringAsync())}");
    }
  }
}