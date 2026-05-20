using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("posts")]
public class Post
{
    [Key]  
    [Column("post_id")] 
    public Guid PostId { get; set; }


    [Column("author_id")]
    public Guid AuthorId { get; set; }


    [Column("title")]
    public String PostTitle {get;set;} = "";


    [Column("preview")]
    public String PostPreview {get;set;} = "";


    [Column("content")]
    public String PostContent {get;set;} = "";


    [Column("is_public")]
    public Boolean IsPublic {get;set;}


    [Column("created_at")]
    public DateTime CreatedAt {get;set;}


    [Column("updated_at")]
    public DateTime UpdateAt {get;set;}

}