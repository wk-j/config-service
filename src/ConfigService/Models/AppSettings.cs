namespace ConfigEditor.Models
{
    public class AppSettings
    {
        public Project[] Projects { set; get; } = new Project[0];
    }

    public class Project
    {
        public string Name { set;get; }
        public string Path { set; get; }
        public string[] Patterns { set; get; } = new string[0];
    }
}