#addin "wk.StartProcess&version=18.5.1"
#addin "wk.ProjectParser"

using PS = StartProcess.Processor;
using ProjectParser;

var npi = EnvironmentVariable("npi");
var name = "ConfigService";

var currentDir = new DirectoryInfo(".").FullName;
var info = Parser.Parse($"src/{name}/{name}.csproj");

Task("Build-Web").Does(() => {
    PS.StartProcess("npm run build", "../ConfigInterface");
});

Task("Pack").Does(() => {
    CleanDirectory($"src/{name}/wwwroot");
    CleanDirectory("publish");
    PS.StartProcess("npm run build", "../ConfigInterface");
    DotNetCorePack($"src/{name}", new DotNetCorePackSettings {
        OutputDirectory = "publish"
    });
});

Task("Publish-NuGet")
    .IsDependentOn("Pack")
    .Does(() => {
        var nupkg = new DirectoryInfo("publish").GetFiles("*.nupkg").LastOrDefault();
        var package = nupkg.FullName;
        Console.WriteLine(nupkg.FullName);
        NuGetPush(package, new NuGetPushSettings {
            Source = "https://www.nuget.org/api/v2/package",
            ApiKey = npi
        });
});

Task("Install")
    .IsDependentOn("Pack")
    .Does(() => {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        PS.StartProcess($"dotnet tool uninstall -g {info.PackageId}");
        PS.StartProcess($"dotnet tool install   -g {info.PackageId}  --add-source {currentDir}/publish --version {info.Version}");
    });

var target = Argument("target", "Pack");
RunTarget(target);