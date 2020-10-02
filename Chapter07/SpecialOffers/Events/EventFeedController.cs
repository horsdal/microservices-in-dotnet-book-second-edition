namespace SpecialOffers.Events
{
  using System.Linq;
  using Microsoft.AspNetCore.Mvc;

  [Route(("/events"))]
  public class EventFeedController : Controller
  {
    private readonly IEventStore eventStore;

    public EventFeedController(IEventStore eventStore)
    {
      this.eventStore = eventStore;
    }
  
    [HttpGet("")]
    public ActionResult<EventFeedEvent[]> GetEvents([FromQuery] int start, [FromQuery] int end)
    {
      if (start < 0 || end < start)
        return BadRequest();

      return this.eventStore.GetEvents(start, end).ToArray();
    }
  }
}