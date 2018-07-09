## Config Editor

[![Build Status](https://travis-ci.org/bcircle-intern/config-service.svg?branch=master)](https://travis-ci.org/bcircle-intern/config-service)
[![Gitter chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/10000-bc/Lobby)

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