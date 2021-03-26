namespace ShoppingCart.EventFeed
{
  using System;

  public record Event(long SequenceNumber, DateTimeOffset OccuredAt, string Name, object Content);
}