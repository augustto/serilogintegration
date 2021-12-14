using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.MSSqlServer.Sinks.MSSqlServer.Options;

namespace SerilogIntegration
{
  public class Program
  {
    public static void Main(string[] args)
    {
      var configuration = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json")
     .Build();
      Log.Logger = new LoggerConfiguration()
          .ReadFrom.Configuration(configuration)
          .WriteTo.Console()
          .WriteTo.MSSqlServer(
          "Server=localhost;Database=NerdStore;User Id=sa;Password=1234ABCD;",
            sinkOptions: new SinkOptions()
            {
                AutoCreateSqlTable = true,
                TableName = "AppLog",
                SchemaName = "dbo",
                BatchPostingLimit = 1000
            }
          )
          .CreateLogger();
      try
      {
        Log.Information("Application starting up");
        CreateHostBuilder(args).Build().Run();
      }
      catch (Exception ex)
      {
        Log.Fatal(ex, "Application start-up failed");
        throw;
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
              webBuilder.UseStartup<Startup>();
            });
  }
}
