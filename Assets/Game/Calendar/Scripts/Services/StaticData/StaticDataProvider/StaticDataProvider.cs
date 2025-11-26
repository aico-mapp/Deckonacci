using Calendar.Scripts.Data.StaticData.Sounds;
using Game.Calendar.Scripts.Data.StaticData;
using UnityEngine;

namespace Game.Calendar.Scripts.Services.StaticData.StaticDataProvider
{
    public class StaticDataProvider : IStaticDataProvider
    {
        private const string SoundDataPath = "StaticData/SoundData";
        private const string GameConfigurationPath = "StaticData/GameSettings";
        
        public SoundData LoadSoundData() => Resources.Load<SoundData>(SoundDataPath);
        public GameSettings LoadGameConfiguration() => Resources.Load<GameSettings>(GameConfigurationPath);
    }
}