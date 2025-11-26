using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Messaging;
using Mode.Scripts.Data;
using Mode.Scripts.Network;
using Newtonsoft.Json.Linq;
using Unity.Notifications.iOS;

namespace Mode.Scripts.Firebase
{
    public class FirebaseService
    {
        private readonly DataService _dataService;
        private readonly RefreshService _refreshService;
        private readonly EventHandler _eventHandler;
        
        private string _statusKey;

        public FirebaseService(DataService dataService,
            RefreshService refreshService, EventHandler eventHandler)
        {
            _dataService = dataService;
            _refreshService = refreshService;
            _eventHandler = eventHandler;
        }

        public void Initialize(string statusKey)
        {
            _statusKey = statusKey;
            FirebaseMessaging.TokenRegistrationOnInitEnabled = true;
            FirebaseMessaging.TokenReceived += OnTokenReceived;
            FirebaseMessaging.MessageReceived += OnMessageReceived;
            _eventHandler.OnLoadEnded += () => FirebaseMessaging.MessageReceived -= OnMessageReceived;
#if UNITY_IOS
            iOSNotificationCenter.ApplicationBadge = 0;
#endif
        }

        public async UniTask ReceiveData(string key) =>
            await FirebaseDatabase.DefaultInstance
                .GetReference(key)
                .GetValueAsync()
                .ContinueWithOnMainThread(ParseSettings);

        public string GetPushStatus()
        {
            var pushStatus = "true";
#if UNITY_IOS
            pushStatus = iOSNotificationCenter
                .GetNotificationSettings()
                .AuthorizationStatus == AuthorizationStatus.Authorized
                ? "true"
                : "false";
#endif
            return pushStatus;
        }

        public async UniTaskVoid TryRefreshToken()
        {
            var token = await FirebaseMessaging.GetTokenAsync();
            await _refreshService.SendRefresh(_dataService.Token, token);
        }

        private void OnTokenReceived(object sender, TokenReceivedEventArgs token)
        {
            _dataService.PushToken = token.Token;
        }
        
        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            var data = args.Message.Data;
            data.TryGetValue(StaticData.NotificationKey, out _dataService.PushLink);
            _eventHandler.InvokeNotificationReceived();
        }

        private void ParseSettings(Task<DataSnapshot> result)
        {
            if (result.IsCompleted == false)
                return;

            var loadedScore = JObject.FromObject(result.Result.Value);
            var isScore = loadedScore.ContainsKey(_statusKey);
            if (!isScore)
                _dataService.SetConfiguration(loadedScore.ToString());
        }
    }
}