using System.Collections.Generic;

namespace Inoreader.Dto
{
    public class SubscriptionsList
    {
        public SubscriptionsList()
        {
            Subscriptions = new List<Subscription>();
        }

        public List<Subscription> Subscriptions { get; set; }
    }

    public class Subscription
    {
        public Subscription()
        {
            Categories = new List<string>();
        }

        public string Id { get; set; }
        
        public string Title { get; set; }
        
        public List<string> Categories { get; set; }
        
        public string SortId { get; set; }
        
        public long FirstItemMsec { get; set; }
        
        public string Url { get; set; }
        
        public string HtmlUrl { get; set; }
        
        public string IconUrl { get; set; }
    }
}