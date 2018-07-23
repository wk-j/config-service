using System.Collections.Generic;
using ConfigEditor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using ConfigEditor.Utility;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using ConfigEditor.Attributes;


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

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public IEnumerable<string> GetProjectNames() {
            return settings.Projects.Select(x => x.Name);
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public string GetProjectPath(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return null;
            }
            var path = project.Path;
            return path;
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public IEnumerable<string> GetProjectSettings(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return Enumerable.Empty<string>();
            }
            if (!Directory.Exists(project.Path)) {
                return Enumerable.Empty<string>();
            }

            var files = project.Patterns
                .Select(x => Directory.EnumerateFiles(project.Path, x, SearchOption.AllDirectories))
                .SelectMany(x => x);
            return files;
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

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public IEnumerable<Node> GetNodes(string path) {
            var project = settings.Projects.FirstOrDefault(x => x.Path == path);
            if (project == null) {
                return Enumerable.Empty<Node>();
            }
            if (!Directory.Exists(project.Path)) {
                return Enumerable.Empty<Node>();
            }
            var dir = new DirectoryInfo(path);
            return FindNode(dir).Append(new Node {
                IsRoot = true,
                Name = dir.Name,
                Parent = 0,
                Id = dir.FullName.GetHashCode(),
                PathFile = dir.FullName.ToString()
            });
        }

        [HttpPost]
        public ActionResult LoginRequest([FromBody] GetLoginRequest request) {
            var user = settings.Login.FirstOrDefault(x => x.User.Equals(request.User) && x.Pass.Equals(request.Pass));
            if (user != null) {
                //  base64 UTF8 (request.User:request.pass)
                var account = $"{request.User}:{request.Pass}";
                var accountBytes = Encoding.UTF8.GetBytes(account);

                var result = new { access_token = Convert.ToBase64String(accountBytes) };
                return Ok(result);

                /*return Ok(new GetLoginRequest {
                    User = request.User,
                    Pass = request.Pass
                });*/
            } else {
                return Unauthorized();
            }
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
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

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
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