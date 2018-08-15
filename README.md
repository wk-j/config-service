## Config Editor

[![Build Status](https://travis-ci.org/bcircle-intern/config-service.svg?branch=master)](https://travis-ci.org/bcircle-intern/config-service)
[![Gitter chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/10000-bc/Lobby)
[![Build status](https://ci.appveyor.com/api/projects/status/1v6aeg32x8adkuba?svg=true)](https://ci.appveyor.com/project/wk-j/config-service)
[![sonarcloud](https://sonarcloud.io/api/project_badges/measure?project=config-service&metric=alert_status)](https://sonarcloud.io/dashboard?id=config-service)

[![](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/images/0)](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/links/0)
[![](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/images/1)](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/links/1)
[![](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/images/2)](https://sourcerer.io/fame/wk-j/bcircle-intern/config-service/links/2)

## Lessons learned

- [ ] [Dependency Injection](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1)
- [ ] [Json Validation](https://github.com/RSuter/NJsonSchema)
- [ ] [Cake](https://cakebuild.net)
- [ ] [NuGet](https://docs.microsoft.com/en-us/nuget/what-is-nuget)
- [ ] [Swagger](https://swagger.io)
- [ ] [Cors](https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-2.1)
- [ ] [EmbeddedFileProvider](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers?view=aspnetcore-2.1)
- [ ] [Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools)
- [ ] [Travis](https://travis-ci.org)
- [ ] [xUnit](https://xunit.github.io)

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