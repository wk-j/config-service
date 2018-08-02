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
using System.Xml.Linq;
using System.Xml;

namespace ConfigEditor.Controllers {

    [Route("api/[controller]/[action]")]
    [ApiController]
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
        public ActionResult<List<string>> GetProjectNames() {
            return Ok(settings.Projects.Select(x => x.Name));
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public ActionResult<GetPath> GetProjectPath(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return NotFound(project);
            }
            var paths = project.Path;
            return new GetPath {
                Path = paths
            };
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public ActionResult<GetFile> GetProjectSettings(string projectName) {
            var project = settings.Projects.FirstOrDefault(x => x.Name == projectName);
            if (project == null) {
                return NotFound(project);
            }
            if (!Directory.Exists(project.Path)) {
                return BadRequest(project.Path);
            }

            var files = project.Patterns
                .Select(x => Directory.EnumerateFiles(project.Path, x, SearchOption.AllDirectories))
                .SelectMany(x => x).ToList();
            var result = files.Where(x => !settings.IgnoreFolder.Any(i => x.Contains(i))).ToList();

            return new GetFile {
                Files = result
            };
        }



        private IEnumerable<Node> FindNode(DirectoryInfo root) {
            var project = settings.Projects.FirstOrDefault(x => root.FullName.Contains(x.Path));
            if (project == null) yield break;

            var files = project.Patterns.Select(pattern => Directory.GetFiles(root.FullName, pattern, SearchOption.TopDirectoryOnly)).SelectMany(x => x);
            foreach (var file in files) {
                var fileInfo = new FileInfo(file);
                if (!settings.IgnoreFolder.Any(x => x.Equals(Path.GetDirectoryName(fileInfo.FullName)))) {
                    var date = fileInfo.LastWriteTimeUtc.AddHours(+7);
                    yield return new Node {
                        IsRoot = false,
                        Id = fileInfo.FullName.GetHashCode(),
                        Name = fileInfo.Name,
                        IsFile = true,
                        Parent = root.FullName.GetHashCode(),
                        PathFile = fileInfo.FullName,
                        ModifieDate = date.ToString("dd/MM/yyyy hh:mm tt"),
                        FileType = Path.GetExtension(fileInfo.FullName)
                    };
                }
            }
            foreach (var item in root.GetDirectories()) {
                if (!settings.IgnoreFolder.Any(x => x.Equals(item.Name))) {
                    var hasMatchFiles = project.Patterns.Any(pattern => Directory.GetFiles(item.FullName, pattern, SearchOption.AllDirectories).Count() > 0);
                    if (hasMatchFiles) {
                        yield return new Node {
                            IsRoot = false,
                            Id = item.FullName.GetHashCode(),
                            Name = item.Name,
                            Parent = root.FullName.GetHashCode(),
                            PathFile = root.FullName,
                            ModifieDate = "",
                            FileType = Path.GetExtension(item.FullName)
                        };
                        foreach (var file in FindNode(item)) {
                            yield return file;
                        }
                    }
                }
            }
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpGet]
        public ActionResult<List<Node>> GetNodes(string path) {
            var project = settings.Projects.FirstOrDefault(x => x.Path == path);
            if (project == null) {
                return NotFound(project);
            }
            if (!Directory.Exists(project.Path)) {
                return BadRequest(project.Path);
            }
            var dir = new DirectoryInfo(path);
            return FindNode(dir).Append(new Node {
                IsRoot = true,
                Name = dir.Name,
                Parent = 0,
                Id = dir.FullName.GetHashCode(),
                PathFile = dir.FullName.ToString(),
                ModifieDate = ""
            }).ToList();
        }

        [HttpPost]
        public ActionResult LoginRequest([FromBody] GetLoginRequest request) {
            var user = settings.Login.FirstOrDefault(x => x.User.Equals(request.User) && x.Pass.Equals(request.Pass));
            if (user != null) {
                //  base64 UTF8 (request.User:request.pass)
                var account = $"{request.User}:{request.Pass}";
                var accountBytes = Encoding.UTF8.GetBytes(account);

                var result = new { AccessToken = Convert.ToBase64String(accountBytes) };
                return Ok(result);
            } else {
                return Unauthorized();
            }
        }

        private string ReformatJson(string content) {
            var obj = JsonConvert.DeserializeObject<dynamic>(content);
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }
        private string ReformatXml(string content) {
            // System.Xml.XmlException
            var bd = new StringBuilder();
            var element = XElement.Parse(content);

            var settings = new XmlWriterSettings {
                Indent = true,
                NewLineOnAttributes = false
            };

            using (var writer = XmlWriter.Create(bd, settings)) {
                element.Save(writer);
            }
            return bd.ToString();
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpPost]
        public ActionResult<DemoContent> ShowDemoContent([FromBody] DemoContentRequest req) {
            var pattern = Path.GetExtension(req.Path);
            if (pattern == ".json") {
                try {
                    var Contents = ReformatJson(req.Content);
                    return new DemoContent {
                        Content = Contents,
                        Pass = true
                    };
                } catch (Newtonsoft.Json.JsonReaderException) {
                    return new DemoContent {
                        Content = "ERROR : Wrong Json Format, Please check again",
                        Pass = false
                    };
                }
            } else if (pattern == ".xml") {
                try {
                    var Contents = ReformatXml(req.Content);
                    return new DemoContent {
                        Content = Contents,
                        Pass = true
                    };
                } catch (System.Xml.XmlException) {
                    return new DemoContent {
                        Content = "ERROR : Wrong Xml Format, Please check again",
                        Pass = false
                    };
                }
            } else {
                System.IO.File.WriteAllText(req.Path, req.Content);
                return new DemoContent {
                    Content = req.Content,
                    Pass = true
                };
            }
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
        [HttpPost]
        public ActionResult<SaveContentResult> SaveSettingContent([FromBody] SaveContentRequest request) {
            var ok = appService.IsAllowToAccess(allowPaths, request.Path);
            if (ok) {
                var pattern = Path.GetExtension(request.Path);
                if (pattern == ".json") {
                    var Content = ReformatJson(request.Content);
                    System.IO.File.WriteAllText(request.Path, Content);
                    return new SaveContentResult {
                        Success = true
                    };
                } else if (pattern == ".xml") {
                    var Content = ReformatXml(request.Content);
                    System.IO.File.WriteAllText(request.Path, Content);
                    return new SaveContentResult {
                        Success = true
                    };
                } else {
                    System.IO.File.WriteAllText(request.Path, request.Content);
                    return new SaveContentResult {
                        Success = true
                    };
                }
            } else {
                return new SaveContentResult {
                    Success = true
                };
            }
        }

        [BasicAuthorize(typeof(BasicAuthorizeFilter))]
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