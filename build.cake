#addin "wk.StartProcess"

using PS = StartProcess.Processor;
Task("Run").Does(() => {
    PS.StartProcess("dotnet run --project src/ConfigService");
});

Task("Watch").Does(() => {
    PS.StartProcess("dotnet watch --project src/ConfigService/ConfigService.csproj run");
});

var target = Argument("target", "Run");
RunTarget(target);