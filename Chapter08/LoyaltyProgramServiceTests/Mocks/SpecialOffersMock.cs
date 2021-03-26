namespace LoyaltyProgramServiceTests.Mocks
{
  using Microsoft.AspNetCore.Mvc;

  public class SpecialOffersMock : ControllerBase
  {
    [HttpGet("/specialoffers/events")]
    public ActionResult<object[]> GetEvents([FromQuery] int start, [FromQuery] int end) =>
      new[]
      {
        new
        {
          SequenceNumber = 1,
          Name = "baz",
          Content = new
          {
            OfferName = "foo",
            Description = "bar",
            Item = new {ProductName = "name"}
          }
        }
      };
  }
}