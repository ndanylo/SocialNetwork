namespace Users.Application.ViewModels
{
    public class UserProfileViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public byte[]? Avatar { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int FriendsCount { get; set; }
        public List<PostViewModel>? Posts { get; set; }
    }
}