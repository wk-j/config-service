#! "netcoreapp2.1"
#r "nuget:Newtonsoft.Json,11.0.0"

using Newtonsoft.Json;

class Settings {
    public string ConnectionString { set; get; }
}

var projects = new string[] { "ProjectA", "ProjectB" };

foreach (var item in projects) {
    var json = JsonConvert.SerializeObject(new Settings { ConnectionString = item });

    var path = Path.Combine("/tmp", item);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    var fullPath = Path.Combine(path, "AppSettings.json");
    File.WriteAllText(fullPath, json);
}