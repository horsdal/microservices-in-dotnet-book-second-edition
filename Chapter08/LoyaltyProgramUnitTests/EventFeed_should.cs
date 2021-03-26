namespace LoyaltyProgramUnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Net.Http;
  using System.Text.Json;
  using System.Threading.Tasks;
  using LoyaltyProgram.EventFeed;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.TestHost;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Hosting;
  using Xunit;

  public class EventFeed_should : IDisposable
  {
    private readonly IHost host;
    private readonly HttpClient sut;

    public EventFeed_should()
    {
      this.host = new HostBuilder()
        .ConfigureWebHost(host =>
          host
            .ConfigureServices(x => x
              .AddScoped<IEventStore, FakeEventStore>()
              .AddControllersByType(typeof(EventFeedController))
              .AddApplicationPart(typeof(EventFeedController).Assembly))
            .Configure(x => x.UseRouting().UseEndpoints(opt => opt.MapControllers()))
            .UseTestServer())
        .Start();
      this.sut = this.host.GetTestClient();
    }

    [Fact]
    public async Task return_events_when_from_event_store()
    {
      var actual = await this.sut.GetAsync("/events?start=0&end=100");

      Assert.Equal(HttpStatusCode.OK, actual.StatusCode);
      var eventFeedEvents =
        await JsonSerializer.DeserializeAsync<IEnumerable<EventFeedEvent>>(await actual.Content.ReadAsStreamAsync())
        ?? Enumerable.Empty<EventFeedEvent>();
      Assert.Equal(100, eventFeedEvents.Count());
    }

    [Fact]
    public async Task return_empty_response_when_there_are_no_more_events()
    {
      var actual = await this.sut.GetAsync("/events?start=200&end=300");

      var eventFeedEvents = await JsonSerializer.DeserializeAsync<IEnumerable<EventFeedEvent>>(await actual.Content.ReadAsStreamAsync());
      Assert.Empty(eventFeedEvents);
    }

    public void Dispose()
    {
      this.sut?.Dispose();
      this.host?.Dispose();
    }
  }

  public class FakeEventStore : IEventStore
  {
    public Task RaiseEvent(string name, object content) =>
      throw new NotImplementedException();

    public Task<IEnumerable<EventFeedEvent>> GetEvents(int start, int end)
    {
      if (start > 100)
        return Task.FromResult(Enumerable.Empty<EventFeedEvent>());

      return Task.FromResult(Enumerable
        .Range(start, end - start)
        .Select(i => new EventFeedEvent(i, DateTimeOffset.UtcNow, "some event", new object())));
    }
  }
}