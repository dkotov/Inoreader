using System.Collections.Generic;

namespace Inoreader.Dto
{
    public class TagsList
    {
        public TagsList()
        {
            Tags = new List<Tag>();
        }

        public List<Tag> Tags { get; set; } 
    }

    public class Tag
    {
        public string Id { get; set; }
        
        public string SortId { get; set; }
    }
}