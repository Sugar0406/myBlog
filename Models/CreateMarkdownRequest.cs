public class CreateMarkdownRequest
{
    public string Title { get; set; } = string.Empty;

    public string Preview { get; set; } = string.Empty;

    public bool PublicSetting { get; set; }
}