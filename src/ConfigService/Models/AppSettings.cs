namespace ConfigEditor.Models {
    public class AppSettings {
        public Project[] Projects { set; get; }
        public Login[] Login { set; get; }
        public string[] IgnoreFolder { set; get; }
    }


}