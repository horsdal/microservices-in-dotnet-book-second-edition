namespace HelloMicroservices
{
  using System;
  using Microsoft.AspNetCore.Mvc;

  public class CurrentDateTimeController
  {
    [HttpGet("/")]
    public object Get() =>  DateTime.UtcNow;
  }
}