using System.Collections.Generic;

namespace Inoreader.Utils.Extensions
{
    public static class StringExt
    {
        public static IEnumerable<string> SplitByLength(this string str, int chunkLength)
        {
            if (str == null) yield break;

            if (str.Length <= chunkLength)
            {
                yield return str;
            }
            else
            {
                yield return str.Substring(0, chunkLength);

                foreach (var chunk in str.Substring(chunkLength).SplitByLength(chunkLength))
                    yield return chunk;
            }
        }
    }
}