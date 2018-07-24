using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ConfigEditor {

    public class Program {

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options => {
                    options.ListenAnyIP(5000);
                })
                .UseStartup<Startup>();

        public static async Task Main(string[] args) {
            var logger = Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.RollingFile("logs/log-{Date}.log")
                .CreateLogger();

            var settings = "appsettings.json";
            if (File.Exists(settings)) {

                var json = File.ReadAllText(settings);
                var errors = await Validator.Schemacheck(json);

                if (errors.Count == 0) {
                    CreateWebHostBuilder(args).Build().Run();
                } else {
                    logger.Error("Invalid configuration (appsettings.json)");
                    foreach (var item in errors) {
                        logger.Error("{0} {1}", item.Kind, item.Property);
                    }
                }
            } else {
                CreateWebHostBuilder(args).Build().Run();
            }
        }
    }
}