using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ConfigEditor.Models;
using ConfigEditor.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle;
using Swashbuckle.AspNetCore.Swagger;

namespace ConfigEditor {
    public class Startup {
        private ILogger<Startup> Logger { get; }

        public Startup(IConfiguration configuration, ILogger<Startup> logger) {
            Configuration = configuration;
            Logger = logger;
            
            
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            var config = Configuration.Get<AppSettings>();
            config.Projects = config.Projects.Select(x => new Project {
                Path = x.Path,
                Patterns = x.Patterns,
                Name = new DirectoryInfo(x.Path).Name
            }).ToArray();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "EditorConfig API", Version = "v1" });
            });

            services
                .AddSingleton<AppSettings>(config)
                .AddSingleton<AppService>(new AppService())
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            var asm = Assembly.GetEntryAssembly();
            var asmName = asm.GetName().Name;
            var defaultOptions = new DefaultFilesOptions();
            defaultOptions.DefaultFileNames.Clear();
            defaultOptions.DefaultFileNames.Add("index.html");
            defaultOptions.FileProvider = 
            new EmbeddedFileProvider(asm, $"{asmName}.wwwroot");
            app
            .UseDefaultFiles(defaultOptions)
            .UseStaticFiles(new StaticFileOptions {
                FileProvider = 
                new EmbeddedFileProvider(asm, $"{asmName}.wwwroot")
            });
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseHsts();
            }

            app.UseCors(builder => {
                builder.AllowAnyHeader();
                builder.AllowAnyMethod();
                builder.AllowAnyOrigin();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
