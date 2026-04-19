using System.ComponentModel.DataAnnotations;


// 變數名稱要和對應form的input的name相同
public class RegisterFormModel: IValidatableObject
{
    // { get; set; } -> 可以讀(get)也可以寫(set）
    public required string Username { get; set; } = "";
    public required string Password { get; set; } = "";
    public required string Email { get; set; } = "";

    [Required(ErrorMessage = "請上傳頭像")]
    public IFormFile? Avatar { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Avatar == null || Avatar.Length == 0)
        {
            yield return new ValidationResult("請上傳頭像", new[] { "Avatar" });
            yield break;
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };

        var extension = Path.GetExtension(Avatar.FileName).ToLower();


        if (!allowedExtensions.Contains(extension) ||
            !allowedTypes.Contains(Avatar.ContentType))
        {
            yield return new ValidationResult("只允許 JPG / PNG / WEBP / GIF 圖片", new[] { "Avatar" });
        }
    }
}