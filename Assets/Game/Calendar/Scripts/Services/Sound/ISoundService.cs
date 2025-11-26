using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Data.StaticData.Sounds;
using Game.Calendar.Scripts.Services.SaveLoad;

namespace Game.Calendar.Scripts.Services.Sound
{
    public interface ISoundService
    {
        bool IsSoundMuted { get; set; }
        float Volume { get; }
        void Construct(ISaveLoad saveLoad, SoundData soundData);
        void PlayBackgroundMusic();
        void PlayEffectSound(SoundId soundId);
        void PlayLongEffectSound(SoundId soundId);
        void SetEffectsVolume(float volume);
        public void MuteSound();
    }
}