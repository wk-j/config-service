using System.Collections.Generic;
using ConfigEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using ConfigEditor.Utility;

namespace ConfigEditor.Controllers {
    [Route("api/[controller]/[action]")]
    public class SearchController : ControllerBase {
        private readonly AppSettings settings;
        private readonly ILogger<SearchController> logger;
        private readonly AppService appService;
        private readonly List<string> allowPaths;

        public SearchController(
                AppSettings settings,
                ILogger<SearchController> logger,
                AppService appService) {
            this.settings = settings;
            this.logger = logger;
            this.appService = appService;

            allowPaths = settings.Projects.Select(x => x.Path).ToList();
        }

        [HttpGet]
        public IEnumerable<string> GetProjectNames() {
            return settings.Projects.Select(x => x.Name);
        }

        [HttpGet]
        public IEnumerable<string> GetProjectSettings(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return Enumerable.Empty<string>();
            } else {
                var files = project.Patterns
                    .Select(x => Directory.EnumerateFiles(project.Path, x, SearchOption.AllDirectories))
                    .SelectMany(x => x);
                return files;
            }
        }

        [HttpGet]
        public dynamic GetSettingContent(string path) {
            var ok = appService.IsAllowToAccess(allowPaths, path);
            if (ok) {
                return new {
                    Success = true,
                    Content = System.IO.File.ReadAllText(path)
                };
            } else {
                return new {
                    Success = false,
                    Content = ""
                };
            }
        }
    }
}