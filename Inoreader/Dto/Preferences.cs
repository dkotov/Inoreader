using System.Collections.Generic;

namespace Inoreader.Dto
{
    public class Preferences
    {
        public Preferences()
        {
            Prefs = new List<Preference>();
        }

        public List<Preference> Prefs { get; set; }
    }
    
    public class Preference
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }
}