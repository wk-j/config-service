 namespace ConfigEditor.Models
{
 public class Project 
 { 
     public string Name { set;get; } 
     public string Path { set; get; } 
     public string[] Patterns { set; get; } = new string[0]; 
 } 
}