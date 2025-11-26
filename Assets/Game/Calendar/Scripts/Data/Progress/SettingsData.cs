using System;
using Calendar.Scripts.Data.Progress;

namespace Game.Calendar.Scripts.Data.Progress
{
    [Serializable]
    public class SettingsData : IPropertyChanged
    {
        public event Action OnPropertyChanged;

        public bool IsSoundMuted
        {
            get => _isSoundMuted;
            set { _isSoundMuted = value; OnPropertyChanged?.Invoke(); }
        }

        private bool _isSoundMuted;
        
        public SettingsData() => IsSoundMuted = false;

    }
}