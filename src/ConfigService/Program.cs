using System;
using System.Collections.Generic;
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
using NJsonSchema;
using NJsonSchema.Validation;
using System.ComponentModel.DataAnnotations;
using ConfigEditor.Models;



namespace ConfigEditor {

    public class Validator {


        public static async Task<ICollection<ValidationError>> Schemacheck(string jsonData) {
            var schema = await JsonSchema4.FromTypeAsync<AppSettings>();
            var errors = schema.Validate(jsonData);
            return errors;
        }
    }

    public class Program {

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>();

        public static async Task Main(string[] args) {
            Log.Logger = new LoggerConfiguration()

                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .WriteTo.Console()
                .WriteTo.RollingFile("logs/log-{Date}.log")
                .CreateLogger();

            var settings = "appsettings.json";
            var json = File.ReadAllText(settings);
            var errors = await Validator.Schemacheck(json);



            switch (errors.Count) {
                case 0:
                    CreateWebHostBuilder(args).Build().Run();
                    break;
                default:

                    Console.WriteLine("Invalid configuration (appsettings.json)");
                    
                    foreach(var item in errors) {
                        Console.WriteLine("{0} {1}", item.Kind, item.Property);
                    }
                    break;

            }
        }

    }

}






