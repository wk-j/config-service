#! "netcoreapp2.1"
#r "nuget:Newtonsoft.Json,11.0.0"

using Newtonsoft.Json;

class Settings {
    public string ConnectionString { set; get; }
    public string AlfrescoUrl { set; get; }
    public string AlfrescoUser { set; get; }
    public string AlfrescoPassword { set; get; }
}

var projects = new string[] { "ProjectA", "ProjectB" };

foreach (var item in projects) {
    var json = JsonConvert.SerializeObject(new Settings { ConnectionString = item });

    var path = Path.Combine("/tmp", item);
    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    foreach (var i in Enumerable.Range(1, 3)) {
        var str = i.ToString("D3");

        foreach (var itemA in new[] { "A1", "A2", "A3" }) {
            var subFolder = Path.Combine(path, itemA);
            if (!Directory.Exists(subFolder)) Directory.CreateDirectory(subFolder);

            var fullPath = Path.Combine(subFolder, $"{str}-AppSettings.json");
            File.WriteAllText(fullPath, json);
        }
    }

}