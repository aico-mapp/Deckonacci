using Calendar.Scripts.Data.StaticData.Sounds;
using Game.Calendar.Scripts.Data.StaticData;

namespace Game.Calendar.Scripts.Services.StaticData
{
    public interface IStaticData : IGlobalService
    {
        SoundData SoundData { get; }
        GameSettings GameSettings { get; }
    }
}