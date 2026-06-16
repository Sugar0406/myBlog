public class UploadImageRequest
{
    public Guid post_id { get; set; }
    public required IFormFile Image { get; set; }
}