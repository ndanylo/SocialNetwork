namespace Users.Application.ViewModels
{
    public class PostViewModel
    {
        public Guid Id { get; set; }
        public UserViewModel User { get; set; } = new UserViewModel();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public byte[]? Image { get; set; }
        public bool LikedByUser { get; set; }
    }
}