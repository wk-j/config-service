using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ConfigEditor.Utility {
    public class AppService {
        public bool IsAllowToAccess(List<string> allows, string path) {
            var fileInfo = new FileInfo(path);
            return allows.Any(x => fileInfo.FullName.Contains(new DirectoryInfo(x).FullName));
        }
    }
}