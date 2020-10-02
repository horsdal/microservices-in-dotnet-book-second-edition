namespace ShoppingCart.EventFeed
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using EventStore.ClientAPI;
  using Newtonsoft.Json;

  public class EsEventStore : IEventStore
  {
    private const string ConnectionString =
      "tcp://admin:changeit@localhost:1113";

    public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
    {
      using var connection = EventStoreConnection.Create(new Uri(ConnectionString));
      await connection.ConnectAsync();
      var result = await connection.ReadStreamEventsForwardAsync(
        "ShoppingCart",
        start: firstEventSequenceNumber,
        count: (int) (lastEventSequenceNumber - firstEventSequenceNumber),
        resolveLinkTos: false);

      return result.Events
        .Select(e =>
          new
          {
            Content = Encoding.UTF8.GetString(e.Event.Data),
            Metadata = JsonConvert.DeserializeObject<EventMetadata>(Encoding.UTF8.GetString(e.Event.Metadata))
          })
        .Select((e, i) =>
          new Event(
            i + firstEventSequenceNumber,
            e.Metadata.OccuredAt,
            e.Metadata.EventName,
            e.Content));
    }

    public async Task Raise(string eventName, object content)
    {
      using var connection = EventStoreConnection.Create(new Uri(ConnectionString));
      await connection.ConnectAsync();
      await connection.AppendToStreamAsync(
        "ShoppingCart",
        ExpectedVersion.Any,
        new EventData(
          Guid.NewGuid(),
          "ShoppingCartEvent",
          isJson: true,
          data: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content)),
          metadata: Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new EventMetadata
          {
            OccuredAt = DateTimeOffset.UtcNow,
            EventName = eventName
          }))));
    }

    public class EventMetadata
    {
      public DateTimeOffset OccuredAt { get; set; }
      public string EventName { get; set; }
    }
  }
}