using System.Collections.Generic;
using System.Linq;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Data.StaticData.Sounds;
using Calendar.Scripts.Services.CoroutineRunner;
using Game.Calendar.Scripts.Services.SaveLoad;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Calendar.Scripts.Services.Sound
{
    public class SoundService : MonoBehaviour, ISoundService, ICoroutineRunner
    {
        public bool IsSoundMuted
        {
            get => _effectsSource.mute;
            set => _effectsSource.mute = _musicSource.mute = value;
        }

        public float Volume => _effectsSource.volume;

        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _effectsSource;
        [SerializeField] private AudioSource _longEffectsSource;
        private Dictionary<SoundId, AudioClipData> _sounds;
        private ISaveLoad _saveLoad;
        private SoundData _soundData;

        public void Construct(ISaveLoad saveLoad, SoundData soundData)
        {
            _saveLoad = saveLoad;
            _soundData = soundData;
            _sounds = soundData.AudioEffectClips.ToDictionary(s => s.Id);
            IsSoundMuted = _saveLoad.Progress.Settings.IsSoundMuted;
        }

        public void MuteSound()
        {
            IsSoundMuted = !IsSoundMuted;
            _saveLoad.Progress.Settings.IsSoundMuted = IsSoundMuted;
            _saveLoad.Progress.SendPropertyChanged();
            Debug.Log($"Sound Muted: {IsSoundMuted}");
        }

        public void PlayBackgroundMusic()
        {
            int day = Random.Range(0, _soundData.BackgroundMusic.Length); //(int)DateTime.Today.DayOfWeek;
            
            if(_soundData.BackgroundMusic != null)
            {
                _musicSource.clip = _soundData.BackgroundMusic[day];
                _musicSource.Play();
            }
        }

        public void PlayEffectSound(SoundId soundId)
        {
            Debug.Log($"PlaySound {soundId}");
            if(_sounds != null)
                _effectsSource.PlayOneShot(_sounds[soundId].Clip);
        }

        public void PlayLongEffectSound(SoundId soundId)
        {
            if (!_longEffectsSource.isPlaying)
                _longEffectsSource.PlayOneShot(_sounds[soundId].Clip);
        }

        public void SetEffectsVolume(float volume) => AudioListener.volume = volume;
    }
}