namespace SpecialOffers.Events
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;

  public interface IEventStore
  {
    void RaiseEvent(string name, object content);
    IEnumerable<EventFeedEvent> GetEvents(int start, int end);
  }

  public class EventStore : IEventStore
  {
    private static long currentSequenceNumber = 0; 
    private static readonly IList<EventFeedEvent> Database = new List<EventFeedEvent>();

    public void RaiseEvent(string name, object content)
    {
      var seqNumber = Interlocked.Increment(ref currentSequenceNumber);
      Database.Add(new EventFeedEvent(seqNumber, DateTimeOffset.UtcNow, name, content));
    }

    public IEnumerable<EventFeedEvent> GetEvents(int start, int end)
      => Database
        .Where(e => start <= e.SequenceNumber && e.SequenceNumber < end)
        .OrderBy(e => e.SequenceNumber);
  }
  
  public class EventFeedEvent
  {
    public long SequenceNumber { get; }
    public DateTimeOffset OccuredAt { get; }
    public string Name { get; }
    public object Content { get; }

    public EventFeedEvent(
      long sequenceNumber,
      DateTimeOffset occuredAt,
      string name,
      object content)
    {
      this.SequenceNumber = sequenceNumber;
      this.OccuredAt = occuredAt;
      this.Name = name;
      this.Content = content;
    }
  }

}