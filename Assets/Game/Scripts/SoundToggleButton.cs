using System;
using Game.Calendar.Scripts.Services.Sound;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class SoundToggleButton : MonoBehaviour
    {
        [SerializeField] private Sprite _soundOnSprite;
        [SerializeField] private Sprite _soundOffSprite;
        
        private Button _button;
        private ISoundService _soundService;
        private Image _image;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        public void Construct(ISoundService soundService)
        {
            _soundService = soundService;
            _button.onClick.AddListener(ToggleSound);
        }
    
        private void OnDestroy()
        {
            _button.onClick.RemoveListener(ToggleSound);
        }

        public void SetImage()
        {
            UpdateImage(_soundService.IsSoundMuted);
        }
    
        public void ToggleSound()
        {
            _soundService.MuteSound();
            SetImage();
        }

        public void UpdateImage(bool isMuted)
        {
            _image.sprite = isMuted ? _soundOffSprite : _soundOnSprite;
        }
    }
}