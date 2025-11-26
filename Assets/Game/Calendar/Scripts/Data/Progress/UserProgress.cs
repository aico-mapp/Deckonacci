using System;
using System.Collections.Generic;
using System.Linq;
using Calendar.Scripts.Data.Progress;
using Newtonsoft.Json;

namespace Game.Calendar.Scripts.Data.Progress
{
    [Serializable]
    public class UserProgress : IPropertyChanged
    {
        public event Action OnPropertyChanged;

        public SettingsData Settings { get; set; }
        
        public bool HadNewItem
        {
            get => _hadNewItem;
            set
            {
                _hadNewItem = value;
                OnPropertyChanged?.Invoke();
            }
        }
        
        private bool _hadNewItem;
        
        public ProfileData ProfileData
        {
            get => _profileData;
            set
            {
                _profileData = value;
                OnPropertyChanged?.Invoke();
            }
        }

        private ProfileData _profileData;
        
        public int CurrentBalance
        {
            get => _currentBalance;
            set
            {
                _currentBalance = value;
                OnPropertyChanged?.Invoke();
            }
        }

        private int _currentBalance;
        
        public BonusWheelData BonusWheelData
        {
            get => _bonusWheelData;
            set
            {
                _bonusWheelData = value;
                OnPropertyChanged?.Invoke();
            }
        }

        private BonusWheelData _bonusWheelData;
        
        public UserProgress()
        {
            Settings = new SettingsData();
            ProfileData = new ProfileData();
            BonusWheelData = new BonusWheelData();

            CurrentBalance = 10000;
        }

        public void Prepare() => Settings.OnPropertyChanged += SendPropertyChanged;

        public void SendPropertyChanged() => OnPropertyChanged?.Invoke();
    }
    
    [Serializable]
    public class BonusWheelData
    {
        public string LastSpinTime { get; set; } = "";
    }
}