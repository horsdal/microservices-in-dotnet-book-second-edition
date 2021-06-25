using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Json;
using ShoppingCart;

CreateHostBuilder(args).Build().Run();

static IHostBuilder CreateHostBuilder(string[] args) =>
  Host.CreateDefaultBuilder(args)
    .UseSerilog((context, logger) =>
    {
      logger
        .Enrich.FromLogContext()
        .Enrich.WithSpan();
      if (context.HostingEnvironment.IsDevelopment())
        logger.WriteTo.ColoredConsole(
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message}{NewLine}{Exception}");
      else
        logger.WriteTo.Console(new JsonFormatter());
    })
    .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
