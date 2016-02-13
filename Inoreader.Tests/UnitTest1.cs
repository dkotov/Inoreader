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
            var inoreader = new Inoreader.Proxy("appId", "appKey");
            inoreader.Authenticate("username", "password");
            Assert.IsNotNull(inoreader.Token);

            var stream = inoreader.GetSubscriptions().First();
            var unreadItems = inoreader.GetItems(stream.Id, filter: ItemsFilterEnum.OnlyUnread, count: 1);
            var feedItem = unreadItems.Items.First();

            //inoreader.MarkAsRead(feedItem.Id);
        }
    }
}