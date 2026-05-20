public class UpdateMarkdownContentRequest
{
    public Guid post_id {get;set;}
    public string UpdateContent {get;set;} = "";
    public DateTime UpdateTime {get;set;}
}