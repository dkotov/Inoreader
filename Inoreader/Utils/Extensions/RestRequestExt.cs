using RestSharp;

namespace Inoreader.Utils.Extensions
{
    public static class RestRequestExt
    {
        public static void AddParameterIfHasValue(this RestRequest request, string parName, string parValue)
        {
            if (!string.IsNullOrEmpty(parValue))
                request.AddParameter(parName, parValue);
        }
    }
}