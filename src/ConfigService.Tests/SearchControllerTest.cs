using System.Threading.Tasks;
using ConfigEditor.Controllers;
using ConfigEditor.Models;
using ConfigEditor.Utility;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Linq;
using System;
using System.IO;

public class SearchControllerTest {

    ILogger<SearchController> logger = new LoggerFactory().CreateLogger<SearchController>();
    AppService service = new AppService();

    [Fact(Skip = "IO")]
    public void GetNodeTest() {
        Console.WriteLine(new DirectoryInfo(".").FullName);
        var settings = new AppSettings {
            Projects = new Project[] {
                new Project {
                    Path = ".",
                    Name = "ProjectA",
                    Patterns = new []  {
                        "appsettings.json"
                    }
                }
            }
        };
        var controller = new SearchController(settings, logger, service);
        var nodes = controller.GetNodes(".");
        Assert.Equal(2, nodes.Value.Count);
    }

    [Fact(Skip = "IO")]
    public void ShouldGetProjectNames() {

        var settings = new AppSettings {
            Projects = new Project[] {
                new Project {
                    Path = "/tmp/ProjectA",
                    Name = "ProjectA",
                    Patterns = new []  {
                        "*.dll"
                    }
                }
            }
        };
        var controller = new SearchController(settings, logger, service);

        var rs = controller.GetProjectNames();
        Assert.Equal(1, rs.Value.Count());
    }

    [Fact(Skip = "IO")]
    public void ShouldGetProjectSettings() {
        var settings = new AppSettings {
            Projects = new Project[] {
                new Project {
                    Path = "/tmp/ProjectA",
                    Name = "ProjectA",
                    Patterns = new []  {
                        "*.json"
                    }
                }
            }
        };
        var controller = new SearchController(settings, logger, service);
        var rs = controller.GetProjectSettings("ProjectA");
        Assert.True(rs.Value.Files.Count() > 0);
    }
}