using System.Linq;
using Inoreader.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inoreader.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var inoreader = new Inoreader.Proxy("username", "password");
            var token = inoreader.Authenticate();
            Assert.IsNotNull(token);

            var stream = inoreader.GetSubscriptions().First();
            var unreadItems = inoreader.GetItems(stream.Id, filter: ItemsFilterEnum.OnlyUnread, count: 1);
            var feedItem = unreadItems.Items.First();

            //inoreader.MarkAsRead(feedItem.Id);
        }
    }
}