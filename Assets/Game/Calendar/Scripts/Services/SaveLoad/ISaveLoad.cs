using Calendar.Scripts.Data.Progress;
using Calendar.Scripts.Services;
using Game.Calendar.Scripts.Data.Progress;

namespace Game.Calendar.Scripts.Services.SaveLoad
{
    public interface ISaveLoad : IGlobalService
    {
        UserProgress Progress { get; set; }
        void Load();
    }
}