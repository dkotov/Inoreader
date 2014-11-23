using System;
using Inoreader.Enum;
using RestSharp;

namespace Inoreader.Utils
{
    public class Helper
    {
        public static Parameter Map(ItemsFilterEnum filter)
        {
            string name;
            object value;

            switch (filter)
            {
                case ItemsFilterEnum.OnlyRead:
                    name = "it"; value = Constants.ItemTag.Read;
                    break;
                case ItemsFilterEnum.OnlyUnread:
                    name = "xt"; value = Constants.ItemTag.Read;
                    break;
                case ItemsFilterEnum.Starred:
                    name = "it"; value = Constants.ItemTag.Starred;
                    break;
                case ItemsFilterEnum.Like:
                    name = "it"; value = Constants.ItemTag.Like;
                    break;

                default:
                    throw new NotSupportedException();
            }

            return new Parameter { Name = name, Value = value };
        }

        public static string UnifyCustomTagName(string tagName)
        {
            return string.IsNullOrEmpty(tagName) || tagName.StartsWith("user/-/", StringComparison.InvariantCultureIgnoreCase)
                ? tagName : string.Format("user/-/label/{0}", tagName);
        }
    }
}