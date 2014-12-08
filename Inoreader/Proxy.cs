using Inoreader.Dto;
using Inoreader.Enum;
using Inoreader.Exceptions;
using Inoreader.Utils;
using Inoreader.Utils.Extensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Inoreader
{
    public class Proxy
    {
        private readonly string _username;
        private readonly string _password;
        
        public string Token { get; private set; }

        public Proxy(string username, string password)
        {
            _username = username;
            _password = password;
        }

        public Proxy(string token)
        {
            Token = token;
        }

        public string Authenticate()
        {
            if (string.IsNullOrEmpty(_username))
                throw new ArgumentException("username");
            if (string.IsNullOrEmpty(_password))
                throw new ArgumentException("password");

            var request = new RestRequest("/accounts/ClientLogin", Method.POST);
            request.AddParameter("Email", _username);
            request.AddParameter("Passwd", _password);

            var response = Execute(request);
            var results = ParseInoreaderResponse(response.Content);

            if (results.ContainsKey("Auth"))
                Token = results["Auth"];

            return Token;
        }

        public UserInfo GetUserInfo()
        {
            var request = new RestRequest("/reader/api/0/user-info");
            return Execute<UserInfo>(request);
        }

        public string ExportSubscriptions()
        {
            var request = new RestRequest("/reader/subscriptions/export");
            return Execute(request).Content;
        }

        public ImportResult Import(string fileName, byte[] bytes)
        {
            var request = new RestRequest("/reader/subscriptions/import", Method.POST);
            request.AddFile("import_file", bytes, fileName);
            return Execute<ImportResult>(request);
        }

        public UnreadCounters GetUnreadCounters()
        {
            var request = new RestRequest("/reader/api/0/unread-count");
            return Execute<UnreadCounters>(request);
        }

        public List<Subscription> GetSubscriptions()
        {
            var request = new RestRequest("/reader/api/0/subscription/list");
            return Execute<SubscriptionsList>(request).Subscriptions;
        }

        public List<Tag> GetTagsAndFolders()
        {
            var request = new RestRequest("/reader/api/0/tag/list");
            return Execute<TagsList>(request).Tags;
        }

        public StreamItems GetItems(string streamId, int? count = null, bool newestFirst = true, long? startTimeUnix = null, ItemsFilterEnum? filter = null)
        {
            var request = new RestRequest("/reader/atom/{streamId}");
            request.AddUrlSegment("streamId", streamId ?? string.Empty);
            if (count.HasValue) request.AddParameter("n", count.Value);
            if (!newestFirst) request.AddParameter("r", "o");
            if (startTimeUnix.HasValue) request.AddParameter("ot", startTimeUnix.Value);
            if (filter.HasValue) request.AddParameter(Helper.Map(filter.Value));
            request.AddQueryParameter("output", "json");

            return Execute<StreamItems>(request);
        }

        public bool RenameTag(string sourceName, string targetName)
        {
            var request = new RestRequest("/reader/api/0/rename-tag", Method.POST);
            request.AddParameter("s", sourceName);
            request.AddParameter("dest", targetName);

            return Execute(request).Content == "OK";
        }

        public bool DeleteTag(string tagName)
        {
            var request = new RestRequest("/reader/api/0/disable-tag", Method.POST);
            request.AddParameter("s", tagName);

            return Execute(request).Content == "OK";
        }

        public void EditCustomTags(string tagToAdd, string tagToRemove, params string[] itemIds)
        {
            EditUnifiedTag(Helper.UnifyCustomTagName(tagToAdd), Helper.UnifyCustomTagName(tagToRemove), itemIds);
        }

        public void EditUnifiedTag(string tagToAdd, string tagToRemove, params string[] itemIds)
        {
            var request = new RestRequest("/reader/api/0/edit-tag", Method.POST);
            foreach (var id in (itemIds ?? new string[0]))
                request.AddParameter("i", id);

            request.AddParameterIfHasValue("a", tagToAdd);
            request.AddParameterIfHasValue("r", tagToRemove);

            Execute(request);
        }

        public void MarkAsRead(params string[] itemIds)
        {
            EditUnifiedTag(Constants.ItemTag.Read, null, itemIds);
        }

        public void MarkAsUnread(params string[] itemIds)
        {
            EditUnifiedTag(null, Constants.ItemTag.Read, itemIds);
        }

        public bool MarkAsRead(string streamIdOrItemStatus, long? onDateMsec = null)
        {
            var request = new RestRequest("/reader/api/0/mark-all-as-read", Method.POST);
            request.AddParameter("s", streamIdOrItemStatus);
            if (onDateMsec.HasValue) request.AddParameter("ts", onDateMsec.Value);

            return Execute(request).Content == "OK";
        }

        public bool MarkAllAsRead()
        {
            return MarkAsRead(Constants.ItemTag.AllUnreadItems);
        }

        public SubscribeResult Subscribe(string url)
        {
            var request = new RestRequest("/reader/api/0/subscription/quickadd", Method.POST);
            request.AddParameter("quickadd", url);
            return Execute<SubscribeResult>(request);
        }

        public void EditSubscription(string streamId, SubscriptionActionEnum action, string newTitle = null, string addToFolder = null, string removeFromFolder = null)
        {
            var request = new RestRequest("/reader/api/0/subscription/edit", Method.POST);
            request.AddParameter("s", streamId);
            request.AddParameter("ac", action.ToString().ToLower());
            request.AddParameterIfHasValue("t", newTitle);
            request.AddParameterIfHasValue("a", Helper.UnifyCustomTagName(addToFolder));
            request.AddParameterIfHasValue("r", Helper.UnifyCustomTagName(removeFromFolder));
        }

        public List<Preference> GetPreferences()
        {
            var request = new RestRequest("/reader/api/0/preference/list");
            return Execute<Preferences>(request).Prefs;
        }

        public List<StreamPreference> GetStreamPreferences()
        {
            var request = new RestRequest("/reader/api/0/preference/stream/list");
            return Execute<StreamPreferences>(request).StreamPrefs;
        }

        public void SetStreamOrdering(string streamId, List<string> sortIds)
        {
            var request = new RestRequest("/reader/api/0/preference/stream/set", Method.POST);
            request.AddParameter("s", streamId);
            request.AddParameter("k", "subscription-ordering");
            request.AddParameter("v", string.Join(string.Empty, sortIds));
            Execute(request);
        }

        #region Private members

        private IRestResponse Execute(IRestRequest request)
        {
            IRestResponse response = null;
            Execute(client => response = client.Execute(request));
            return response;
        }

        private T Execute<T>(IRestRequest request) where T : new()
        {
            IRestResponse<T> response = null;
            Execute(client => response = client.Execute<T>(request));
            return response.Data;
        }

        private void Execute(Func<RestClient, IRestResponse> runRequest)
        {
            var client = new RestClient(Constants.BaseUrl);
            if (!string.IsNullOrEmpty(Token))
                client.Authenticator = new GoogleAuthenticator(Token);

            var response = runRequest(client);
            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response. Check inner details for more info.";
                throw new InoreaderProxyException(message, response.ErrorException);
            }

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var parsedResponse = ParseInoreaderResponse(response.Content);
                var errorMsg = parsedResponse.ContainsKey("Error") ? parsedResponse["Error"] : response.Content;
                throw new InoreaderApiException(response.StatusCode, errorMsg);
            }
        }

        private static Dictionary<string, string> ParseInoreaderResponse(string response)
        {
            return (response ?? string.Empty).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Split('='))
                .Where(x => x.Length == 2)
                .ToDictionary(x => x[0], x => x[1] == "null" ? null : x[1]);
        }

        #endregion
    }
}
