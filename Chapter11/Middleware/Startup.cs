namespace Middleware
{
  using System;
  using System.Threading.Tasks;
  using Microsoft.AspNetCore.Builder;
  using Microsoft.AspNetCore.Hosting;
  using Microsoft.AspNetCore.Http;

  public class Startup
  {
    public void Configure(IApplicationBuilder app)
    {
      app.Use(next => ctx =>
        {
          Console.WriteLine("Got request in lambda middleware");
          return next(ctx);
        })
        .Use(next => new ConsoleMiddleware(next).Invoke)
        .UseMiddleware<ConsoleMiddleware>()
        .UseMiddleware<RedirectingMiddleware>()
        .Use(next => ctx => Task.CompletedTask);
    }
  }

  public class ConsoleMiddleware
  {
    private readonly RequestDelegate next;

    public ConsoleMiddleware(RequestDelegate next) => this.next = next;

    public Task Invoke(HttpContext ctx)
    {
      Console.WriteLine("Got request in class middleware");
      return this.next(ctx);
    }
  }

  public class RedirectingMiddleware
  {
    private readonly RequestDelegate next;

    public RedirectingMiddleware(RequestDelegate next) => this.next = next;

    public Task Invoke(HttpContext ctx)
    {
      switch (ctx.Request.Path.Value?.TrimEnd('/'))
      {
        case "/oldpath":
        {
          ctx.Response.Redirect("/newpath", permanent: true);
          return Task.CompletedTask;
        }
        default:
          return this.next(ctx);
      }
    }
  }
}