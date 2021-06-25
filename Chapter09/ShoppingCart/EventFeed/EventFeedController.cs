namespace ShoppingCart.EventFeed
{
  using System.Linq;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Mvc;

  [Route("/events")]
  public class EventFeedController
  {
    private readonly IEventStore eventStore;

    public EventFeedController(IEventStore eventStore) => this.eventStore = eventStore;
    
    [HttpGet("")]
    public async Task<Event[]> Get([FromQuery] long start, [FromQuery] long end = long.MaxValue) =>
      (await this.eventStore.GetEvents(start, end)).ToArray();
  }
}