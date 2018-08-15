## Command

```bash
dotnet tool install -g dotnet-sonarscanner
dotnet-sonarscanner begin /k:"config-service" /d:sonar.organization="wk-j-github" /d:sonar.host.url="https://sonarcloud.io" /d:sonar.login="940f6517a5a823b166a31a9173557dedbdbc9401"
dotnet build src/ConfigService/ConfigService.csproj
dotnet-sonarscanner end /d:sonar.login="940f6517a5a823b166a31a9173557dedbdbc9401"
```