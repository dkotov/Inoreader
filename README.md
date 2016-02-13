## .NET wrapper for [Inoreader](http://inoreader.com) API 

See Inoreader official API documentation [here](http://www.inoreader.com/developers).

### Installation

Install [NuGet package](http://www.nuget.org/packages/Inoreader) by running following command in Package Manager Console:
```
PM> Install-Package Inoreader
```

### Usage

```c#
var inoreader = new Inoreader.Proxy("appId", "appKey");

// 1. authenticate with username/password
inoreader.Authenticate("username", "password");
inoreader.Token; // => "G2UlCa...Fx"

// 2. authenticate with token
inoreader.Authenticate("yourtoken");
inoreader.Token; // => "yourtoken"

// 3. get user info
var user = inoreader.GetUserInfo();
user.UserId; // => "1005921515"
user.UserName; // => "Jacket"

// 4. get subscriptions list
var subscriptions = inoreader.GetSubscriptions();
var streamId = subscriptions.First().Id;

// 5. get feed unread items
var unreadItems = inoreader.GetItems(stream.Id, filter: ItemsFilterEnum.OnlyUnread, count: 1);

var feedItem = unreadItems.Items.First();
feedItem.Title; // => "Largest viral genome yet carries 2,300 genes that are new to biology"
feedItem.Author; // => "John Timmer"
feedItem.Canonical.Single().Href; // => "http://feeds.arstechnica.com/~r/arstechnica/science/~3/JvoygbfT84Y/story01.htm"

// 6. mark item as read
var success = inoreader.MarkAsRead(feedItem.Id); // success => true

// etc.
```