using System.Collections.Generic;

namespace Inoreader.Dto
{
    public class StreamItems
    {
        public StreamItems()
        {
            Self = new Source();
            Items = new List<Item>();
        }

        public string Direction { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Source Self { get; set; }
        public long Updated { get; set; }
        public List<Item> Items { get; set; }
        public string Continuation { get; set; }
    }

    public class Item
    {
        public Item()
        {
            Categories = new List<string>();
            Canonical = new List<Source>();
            Alternate = new List<TypedSource>();
            Summary = new ItemSummary();
            Origin = new ItemOrigin();
        }

        public long CrawlTimeMsec { get; set; }
        public long TimestampUsec { get; set; }
        public string Id { get; set; }
        public List<string> Categories { get; set; }
        public string Title { get; set; }
        public long Published { get; set; }
        public long Updated { get; set; }
        public List<Source> Canonical { get; set; }
        public List<TypedSource> Alternate { get; set; }
        public ItemSummary Summary { get; set; }
        public string Author { get; set; }
        public ItemOrigin Origin { get; set; }
    }

    public class Source
    {
        public string Href { get; set; }
    }

    public class TypedSource : Source
    {
        public string Type { get; set; }
    }

    public class ItemSummary
    {
        public string Direction { get; set; }
        public string Content { get; set; }
    }

    public class ItemOrigin
    {
        public string StreamId { get; set; }
        public string Title { get; set; }
        public string HtmlUrl { get; set; }
    }
}