
namespace ConfigEditor.Content {
    public class SaveContentRequest { 
     public string Path { set; get; } 
     public string Content { set; get; } 
 } 
  
 public class GetContentResult { 
     public string Path { set; get; } 
     public string Content { set; get; } 
     public bool Success { set; get; } 
 } 
  
 public class SaveContentResult { 
     public bool Success { set; get; } 
 } 
}