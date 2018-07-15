using System.Collections.Generic;
using ConfigEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using ConfigEditor.Utility;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

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
            }
            var files = project.Patterns
                .Select(x => Directory.EnumerateFiles(project.Path, x, SearchOption.AllDirectories))
                .SelectMany(x => x);
            return files;
        }
        private IEnumerable<Node> FindNode(DirectoryInfo root) {
            foreach (var file in root.GetFiles()) {
                if (file.Name.ToLower().EndsWith(".json")) {
                    yield return new Node {
                        IsRoot = false,
                        Id = file.FullName.GetHashCode(),
                        Name = file.Name,
                        IsFile = true,
                        Parent = root.FullName.GetHashCode()
                    };
                }
            }

            foreach (var item in root.GetDirectories()) {
                yield return new Node {
                    IsRoot = false,
                    Id = item.FullName.GetHashCode(),
                    Name = item.Name,
                    Parent = root.FullName.GetHashCode()
                };

                foreach (var file in FindNode(item)) {
                    yield return file;
                }
            }
        }

         [HttpGet]
        public IEnumerable<Node> GetNodes(string path) {
             var project = settings.Projects.FirstOrDefault(x => x.Path == path);
            if (project == null) {
                return Enumerable.Empty<Node>();
            }
            var dir = new DirectoryInfo(path);
            return FindNode(dir).Append(new Node {
                IsRoot = true,
                Name = dir.Name,
                Parent = 0,
                Id = dir.FullName.GetHashCode()
            });   
        }

        [HttpPost]
        public SaveContentResult SaveSettingContent([FromBody] SaveContentRequest request) {
            var ok = appService.IsAllowToAccess(allowPaths, request.Path);

            if (ok) {
                System.IO.File.WriteAllText(request.Path, request.Content);
                return new SaveContentResult {
                    Success = true
                };
            } else {
                return new SaveContentResult {
                    Success = true
                };
            }
        }

        [HttpGet]
        public GetContentResult GetSettingContent(string path) {
            var ok = appService.IsAllowToAccess(allowPaths, path);
            if (ok) {
                return new GetContentResult {
                    Success = true,
                    Path = path,
                    Content = System.IO.File.ReadAllText(path)
                };
            } else {
                return new GetContentResult {
                    Success = false,
                    Path = path,
                    Content = ""
                };
            }
        }
    }
}