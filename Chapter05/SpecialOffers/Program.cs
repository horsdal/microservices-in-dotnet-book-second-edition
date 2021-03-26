  using Microsoft.AspNetCore.Hosting;
  using Microsoft.Extensions.Hosting;
  using SpecialOffers;

  CreateHostBuilder(args).Build().Run();

  static IHostBuilder CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args)
      .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
  }
