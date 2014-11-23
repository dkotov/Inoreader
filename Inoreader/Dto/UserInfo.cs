namespace Inoreader.Dto
{
    public class UserInfo
    {
        public string UserId { get; set; }
        
        public string UserName { get; set; }
        
        public string UserProfileId { get; set; }
        
        public string UserEmail { get; set; }
        
        public bool IsBloggerUser { get; set; }
        
        public long SignupTimeSec { get; set; }
        
        public bool IsMultiLoginEnabled { get; set; }
    }
}