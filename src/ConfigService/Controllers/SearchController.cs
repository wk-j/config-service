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
        public ActionResult GetProjectNames() {
            return Ok(settings.Projects.Select(x => x.Name));
        }

        [HttpGet]
        public ActionResult GetProjectPath(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return NotFound(project);
            }
            var path = project.Path;
            return Ok(path);
        }

        [HttpGet]
        public ActionResult GetProjectSettings(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return  NotFound(project);
            }
            if (!Directory.Exists(project.Path)) {
                return  BadRequest(project.Path);
            }

            var files = project.Patterns
                .Select(x => Directory.EnumerateFiles(project.Path, x, SearchOption.AllDirectories))
                .SelectMany(x => x);
            return Ok(files);
        }

        private IEnumerable<Node> FindNode(DirectoryInfo root) {
            var project = settings.Projects.FirstOrDefault(x => root.FullName.Contains(x.Path));
            if (project == null) yield break;

            var files = project.Patterns.Select(pattern => Directory.GetFiles(root.FullName, pattern, SearchOption.TopDirectoryOnly)).SelectMany(x => x);
            foreach (var file in files) {
                var fileInfo = new FileInfo(file);
                yield return new Node {
                    IsRoot = false,
                    Id = fileInfo.FullName.GetHashCode(),
                    Name = fileInfo.Name,
                    IsFile = true,
                    Parent = root.FullName.GetHashCode(),
                    PathFile = fileInfo.FullName
                };
            }
            foreach (var item in root.GetDirectories()) {
                var hasMatchFiles = project.Patterns.Any(pattern => Directory.GetFiles(item.FullName, pattern, SearchOption.AllDirectories).Count() > 0);
                if (hasMatchFiles) {
                    yield return new Node {
                        IsRoot = false,
                        Id = item.FullName.GetHashCode(),
                        Name = item.Name,
                        Parent = root.FullName.GetHashCode(),
                        PathFile = root.FullName
                    };
                    foreach (var file in FindNode(item)) {
                        yield return file;
                    }
                }
            }
        }

        [HttpGet]
        public ActionResult<Node> GetNodes(string path) {
            var project = settings.Projects.FirstOrDefault(x => x.Path == path);
            if (project == null) {
                return NotFound(project);
            }
            if (!Directory.Exists(project.Path)) {
                return BadRequest(project.Path);
            }
            var dir = new DirectoryInfo(path);
            return Ok(FindNode(dir).Append(new Node {
                IsRoot = true,
                Name = dir.Name,
                Parent = 0,
                Id = dir.FullName.GetHashCode(),
                PathFile = dir.FullName.ToString()
            }));
        }

        [HttpPost]
        public ActionResult<GetLoginRequest> LoginRequest([FromBody] GetLoginRequest request) {
            var user = settings.Login.FirstOrDefault(x => x.User.Equals(request.User) && x.Pass.Equals(request.Pass));
            if (user != null) {
                return new GetLoginRequest {
                    User = request.User,
                    Pass = request.Pass
                };
            } else {
                return Unauthorized();
            }
        }

        [HttpPost]
        public ActionResult<SaveContentResult> SaveSettingContent([FromBody] SaveContentRequest request) {
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
        public ActionResult<GetContentResult> GetSettingContent(string path) {
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