namespace LoyaltyProgramServiceTests.Mocks
{
  using Microsoft.AspNetCore.Mvc;

  public class NotificationsMock : ControllerBase
  {
    public static bool ReceivedNotification = false;

    [HttpPost("/notify")]
    public OkResult Notify()
    {
      ReceivedNotification = true;
      return Ok();
    }
  }
}