namespace ShoppingCart.EventFeed
{
  using System.Linq;
  using Microsoft.AspNetCore.Mvc;

  [Route("/events")]
  public class EventFeedController
  {
    private readonly IEventStore eventStore;

    public EventFeedController(IEventStore eventStore) => this.eventStore = eventStore;
    
    [HttpGet("")]
    public Event[] Get([FromQuery] long start, [FromQuery] long end = long.MaxValue) => 
      this.eventStore.GetEvents(start, end).ToArray();
  }
}