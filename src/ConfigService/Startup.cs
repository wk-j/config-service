using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ConfigEditor.Models;
using ConfigEditor.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle;
using Swashbuckle.AspNetCore.Swagger;

namespace ConfigEditor {
    public class BrotliCompressionProvider : ICompressionProvider {
        public string EncodingName => "br";
        public bool SupportsFlush => true;
        public Stream CreateStream(Stream outputStream) {
            return new System.IO.Compression.BrotliStream(outputStream, CompressionMode.Compress);
        }
    }
    public class Startup {
        private ILogger<Startup> Logger { get; }

        public Startup(IConfiguration configuration, ILogger<Startup> logger) {
            Configuration = configuration;
            Logger = logger;


        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            var config = Configuration.Get<AppSettings>();

            if (config.Login == null) {
                config.Login = new Login[]{
                    new Login(){
                    User = "admin",
                    Pass = "admin"}
                };
            }

            if (config.Projects == null) {
                string path = System.Environment.CurrentDirectory;
                config.Projects = new Project[]{
                    new Project(){
                        Path = path,
                        Patterns = new string[] { "*.config", "*.json", "*.properties" },
                        Name = new DirectoryInfo(path).Name
                    }
                };
            }

            if (config.IgnoreFolder == null) {
                config.IgnoreFolder = new string[] {
                    "node_modules",
                    "webapps",
                    ".git",
                    "packages",
                    "target",
                    "dist"
                };
            }

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            services.AddResponseCompression(options => {
                options.Providers.Add<BrotliCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] {
                        "image/svg+xml",
                        "application/javascript"
                    });
            });
            services
                .AddSingleton<AppSettings>(config)
                .AddSingleton<AppService>(new AppService())
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {

            if (env.IsProduction()) {
                var asm = Assembly.GetEntryAssembly();
                var asmName = asm.GetName().Name;
                var defaultOptions = new DefaultFilesOptions();
                defaultOptions.DefaultFileNames.Clear();
                defaultOptions.DefaultFileNames.Add("index.html");
                defaultOptions.FileProvider = new EmbeddedFileProvider(asm, $"{asmName}.wwwroot");

                app.UseDefaultFiles(defaultOptions);
                app.UseResponseCompression();
                app.UseStaticFiles(new StaticFileOptions {
                    FileProvider = new EmbeddedFileProvider(asm, $"{asmName}.wwwroot")
                });
                app.UseDeveloperExceptionPage();

            } else {
                app.UseDefaultFiles();
                app.UseResponseCompression();
                app.UseStaticFiles();
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
