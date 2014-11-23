namespace Inoreader
{
    public static class Constants
    {
        public const string BaseUrl = "https://www.inoreader.com";

        public class Stream
        {
            public const string Root = "user/-/state/com.google/root";
        }

        public class ItemTag
        {
            public const string Read = "user/-/state/com.google/read";
            public const string Starred = "user/-/state/com.google/starred";
            public const string Like = "user/-/state/com.google/like";
            public const string Broadcast = "user/-/state/com.google/broadcast";
            public const string AllUnreadItems = "user/-/state/com.google/reading-list";
        }
    }
}