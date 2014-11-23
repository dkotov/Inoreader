using System.Collections.Generic;
using System.Linq;
using Inoreader.Utils.Extensions;

namespace Inoreader.Dto
{
    public class StreamPreferences
    {
        public StreamPreferences()
        {
            StreamPrefs = new List<StreamPreference>();
        }

        public List<StreamPreference> StreamPrefs { get; set; }
    }

    public class StreamPreference
    {
        public string Id { get; set; }
        public string Value { get; set; }

        public List<string> SortIds
        {
            get
            {
                return Id == "subscription-ordering"
                    ? (Value ?? string.Empty).SplitByLength(8).ToList()
                    : null;
            }
        }
    }
}