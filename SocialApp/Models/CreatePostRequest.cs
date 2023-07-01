namespace SocialApp.Models;

public class CreatePostRequest
{
    public string HtmlContent { get; set; }
    public IFormFile? Image { get; set; }
}