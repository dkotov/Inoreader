namespace Inoreader.Dto
{
    public class SubscribeResult
    {
        public string Query { get; set; }
        public int NumResults { get; set; }
        public string StreamId { get; set; }

        public bool Success
        {
            get { return NumResults > 0; }
        }
    }
}