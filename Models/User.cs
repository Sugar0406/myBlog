using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    [Key]  //指定這是PK
    [Column("user_id")] //mapping database欄位
    public Guid UserId { get; set; }

    [Column("user_name")]
    public string Username { get; set; } = "";

    [Column("email_account")]
    public string Email { get; set; } = "";

    [Column("hash_password")]
    public string PasswordHash { get; set; } = "";

    [Column("user_avatar_path")]
    public string AvatarPath { get; set; } = "";
}