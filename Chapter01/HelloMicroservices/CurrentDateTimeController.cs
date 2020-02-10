namespace HelloMicroservices
{
  using System;
  using Microsoft.AspNetCore.Mvc;

  public class CurrentDateTimeController : Controller
  {
    [HttpGet("/")]
    public object Get() => DateTime.UtcNow; 
  }
}