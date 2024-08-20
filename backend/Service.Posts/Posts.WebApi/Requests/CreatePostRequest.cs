namespace Posts.WebApi.Requests
{
    public class CreatePostRequest
    {
        public string Content { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}
