using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

var start = 0; // get from a database
var end = 100;
var client = new HttpClient();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
var resp = await client.GetAsync(new Uri($"http://special-offers:5002/events?start={start}&end={end}"));
Console.WriteLine(await resp.Content.ReadAsStringAsync());