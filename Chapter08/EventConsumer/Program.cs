using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

var start = 0; // get from a database
var end = 100;
var specialOffersHostName = args.Length >= 1 ? args[0] : "http://special-offers:5002";
var notificationsHostName = args.Length >= 2 ? args[1] : "http://notificatoins:5003";

await EventConsumer.ConsumeBatch(start, end, specialOffersHostName, notificationsHostName);


public static class EventConsumer
{
  public static async Task ConsumeBatch(int start, int end, string specialOffersHostName, string notificationsHostName)
  {
    var client = new HttpClient();
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    var resp = await client.GetAsync(new Uri($"{specialOffersHostName}/events?start={start}&end={end}"));

    var events = await JsonSerializer.DeserializeAsync<dynamic[]>(await resp.Content.ReadAsStreamAsync()) ?? Array.Empty<dynamic>();

    foreach (var @event in events)
    {
      // Match special offer in @event to registered users and send notification to matching user.
      await client.PostAsync($"{notificationsHostName}/notify", new StringContent(""));
    }
  }
}
