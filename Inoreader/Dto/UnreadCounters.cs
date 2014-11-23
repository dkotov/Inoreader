using System.Collections.Generic;
using RestSharp.Deserializers;

namespace Inoreader.Dto
{
    public class UnreadCounters
    {
        public UnreadCounters()
        {
            UnreadCounts = new List<StreamUnreadCounter>();
        }

        public int Max { get; set; }

        public List<StreamUnreadCounter> UnreadCounts { get; set; }
    }

    public class StreamUnreadCounter
    {
        [DeserializeAs(Name = "id")]
        public string StreamId { get; set; }
        
        public int Count { get; set; }

        public long NewestItemTimestampUsec { get; set; }
    }
}