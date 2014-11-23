using RestSharp.Deserializers;

namespace Inoreader.Dto
{
    public class ImportResult
    {
        public int Status { get; set; }
        
        public int Imported { get; set; }
        
        public int TimeSec { get; set; }

        [DeserializeAs(Name = "error_msg")]
        public string ErrorMsg { get; set; }

        public bool Success
        {
            get { return Status > 0; }
        }
    }
}