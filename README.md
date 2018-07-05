## Backend

[![Build Status](https://travis-ci.org/bcircle-intern/config-service.svg?branch=master)](https://travis-ci.org/bcircle-intern/config-service)
[![Gitter chat](https://badges.gitter.im/gitterHQ/gitter.png)](https://gitter.im/10000-bc/Lobby)

```bash
dotnet restore src/ConfigEditor/ConfigEditor.csproj
dotnet run --project src/ConfigEditor/ConfigEditor.csproj
```

## Create fake settings

```bash
dotnet-script scripts/CreateFakeSettings.csx
```

## Swagger UI

- http://localhost:5000/swagger