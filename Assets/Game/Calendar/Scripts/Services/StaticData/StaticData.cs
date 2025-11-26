using Calendar.Scripts.Data.StaticData.Sounds;
using Game.Calendar.Scripts.Data.StaticData;
using Game.Calendar.Scripts.Services.StaticData.StaticDataProvider;

namespace Game.Calendar.Scripts.Services.StaticData
{
    public class StaticData : IStaticData
    {
        public SoundData SoundData { get; private set; }
        public GameSettings GameSettings { get; private set; }
        
        private readonly IStaticDataProvider _staticDataProvider;

        public StaticData(IStaticDataProvider staticDataProvider)
        {
            _staticDataProvider = staticDataProvider;
            LoadStaticData();
        }

        public void LoadStaticData()
        {
            SoundData = _staticDataProvider.LoadSoundData();
            GameSettings = _staticDataProvider.LoadGameConfiguration();
        }
    }
}