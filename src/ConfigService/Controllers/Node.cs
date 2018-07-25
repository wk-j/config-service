namespace ConfigEditor.Controllers {
    public class Node {
        public bool IsRoot { set; get; }
        public bool IsFile { set; get; }
        public int Id { set; get; }
        public string Name { set; get; }
        public int Parent { set; get; }
        public string PathFile { set; get; }
        public string ModifieDate { set; get; }
        public string FileType { set; get; }
    }
}