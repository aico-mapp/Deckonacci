using Calendar.Scripts.Data.StaticData.Sounds;
using Game.Calendar.Scripts.Data.StaticData;

namespace Game.Calendar.Scripts.Services.StaticData.StaticDataProvider
{
    public interface IStaticDataProvider : IGlobalService
    {
        SoundData LoadSoundData();
        GameSettings LoadGameConfiguration();
    }
}