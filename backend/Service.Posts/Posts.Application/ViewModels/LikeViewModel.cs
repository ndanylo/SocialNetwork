namespace Posts.Application.ViewModels
{
    public class LikeViewModel
    {
        public Guid PostId { get; set; }
        public UserViewModel? User { get; set; }
    }
}