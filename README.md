## Config Editor

[![Build Status](https://travis-ci.org/bcircle-intern/config-service.svg?branch=master)](https://travis-ci.org/bcircle-intern/config-service)
[![Gitter chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/10000-bc/Lobby)

## Lessons learned

- [ ] [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)
- [ ] [Json Validation](https://github.com/RSuter/NJsonSchema)
- [ ] [Cake](https://cakebuild.net)
- [ ] [NuGet](https://docs.microsoft.com/en-us/nuget/what-is-nuget)
- [ ] [Swagger](https://swagger.io)
- [ ] [Cors](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1)
- [ ] [EmbeddedFileProvider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-2.1)
- [ ] [Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)

## Installation

```bash
dotnet tool install -g BCircle.EditorConfig
```

## Start

```bash
bcircle-config-editor
```


## Development

Create fake configs

```bash
dotnet-script scripts/CreateFakeSettings.csx
```

Run

```bash
dotnet restore src/ConfigEditor/ConfigEditor.csproj
dotnet run --project src/ConfigEditor/ConfigEditor.csproj
```

Swagger

- http://localhost:5000/swagger