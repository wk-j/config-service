using System.Threading.Tasks;
using ConfigEditor.Controllers;
using ConfigEditor.Models;
using ConfigEditor.Utility;
using Microsoft.Extensions.Logging;
using Xunit;
using System.Linq;
using System;

public class SearchControllerTest {

    ILogger<SearchController> logger = new LoggerFactory().CreateLogger<SearchController>();
    AppService service = new AppService();

    [Fact]
    public void ShouldGetProjectNames() {

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

        var rs = controller.GetProjectNames();
        Assert.Equal(1, rs.Count());
    }

    [Fact(Skip="IO")]
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
        Assert.True(rs.Count() > 0);
    }
}