using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class UserPageViewModel
{
    public User User { get; set; } = null!;

    public List<Post> Posts { get; set; } = new();
}