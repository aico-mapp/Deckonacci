using Mode.Scripts.Firebase;

namespace Mode.Scripts.Data
{
    public class LoadedProjectData
    {
        private readonly FirebaseService _firebaseService;
        private readonly DataService _dataService;

        public string Host;
        public string PushReceive;
        public bool IsPushStatusChanged;

        public LoadedProjectData(FirebaseService firebaseService,
            DataService dataService)
        {
            _firebaseService = firebaseService;
            _dataService = dataService;
        }

        public void SetPushStatus()
        {
            PushReceive = _firebaseService.GetPushStatus();
            var cachedStatus = _dataService.PushStatus;
            IsPushStatusChanged = string.IsNullOrEmpty(cachedStatus) ||
                                  string.CompareOrdinal(cachedStatus, PushReceive) != 0;
        }
    }
}