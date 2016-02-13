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
        private readonly string _appId;
        private readonly string _appKey;

        public string Token { get; private set; }

        public Proxy(string appId, string appKey)
        {
            if (string.IsNullOrEmpty(appId))
                throw new ArgumentNullException("appId");
            if (string.IsNullOrEmpty(appKey))
                throw new ArgumentNullException("appKey");

            _appId = appId;
            _appKey = appKey;
        }

        public void Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("username");
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("password");

            var request = new RestRequest("/accounts/ClientLogin", Method.POST);
            request.AddParameter("Email", username);
            request.AddParameter("Passwd", password);

            var response = Execute(request);
            var results = ParseInoreaderResponse(response.Content);

            if (results.ContainsKey("Auth"))
                Authenticate(results["Auth"]);
        }

        public void Authenticate(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token");

            Token = token;
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

        public bool MarkAsReadByFilter(string streamIdOrItemStatus, long? onDateMsec = null)
        {
            var request = new RestRequest("/reader/api/0/mark-all-as-read", Method.POST);
            request.AddParameter("s", streamIdOrItemStatus);
            if (onDateMsec.HasValue) request.AddParameter("ts", onDateMsec.Value);

            return Execute(request).Content == "OK";
        }

        public bool MarkAllAsRead()
        {
            return MarkAsReadByFilter(Constants.ItemTag.AllUnreadItems);
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
            var response = Execute<IRestResponse>(request, client => client.Execute);
            return response;
        }

        private T Execute<T>(IRestRequest request) where T : new()
        {
            var response = Execute<IRestResponse<T>>(request, client => client.Execute<T>);
            return response.Data;
        }

        private TResponse Execute<TResponse>(IRestRequest request, Func<RestClient, Func<IRestRequest, TResponse>> runRequest) where TResponse : IRestResponse
        {
            request.AddHeader("AppId", this._appId);
            request.AddHeader("AppKey", this._appKey);

            var client = new RestClient(Constants.BaseUrl);
            if (!string.IsNullOrEmpty(Token))
                client.Authenticator = new GoogleAuthenticator(Token);

            var response = runRequest(client)(request);
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

            return response;
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
