using Calendar.Scripts.Data.Progress;
using Calendar.Scripts.Extensions;
using Game.Calendar.Scripts.Data.Progress;
using Game.Calendar.Scripts.Extensions;
using UnityEngine;

namespace Game.Calendar.Scripts.Services.SaveLoad
{
    public class SaveLoad : ISaveLoad
    {
        public UserProgress Progress { get; set; }
        private const string ProgressKey = "Progress";

        public void Load()
        {
            string progressJson = PlayerPrefs.GetString(ProgressKey);
            Progress = progressJson.ToDeserialized<UserProgress>() ?? new UserProgress();
            Progress.Prepare();
            Progress.OnPropertyChanged += SaveProgress;
            Debug.Log($"Load Progress - {Progress.ToJson()}");
        }

        private void SaveProgress()
        {
            PlayerPrefs.SetString(ProgressKey, Progress.ToJson());
        }
    }
}