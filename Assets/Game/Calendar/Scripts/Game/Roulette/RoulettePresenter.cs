using Calendar.Scripts.Data.Enums;
using Game.Calendar.Scripts.Services.Sound;

namespace Game.Calendar.Scripts.Game.UI.RouletteScreen
{
    public class RoulettePresenter
    {
        private readonly RouletteView _view;
        private readonly ISoundService _soundService;

        public RoulettePresenter(RouletteView view, ISoundService soundService)
        {
            _view = view;
            _soundService = soundService;
            
            _view.Construct(soundService);
        }

        public void Enable()
        {
            _view.SubscribeView();
            _view.OnBackClick.AddListener(OnBackClicked);
        }

        public void Disable()
        {
            _view.UnsubscribeView();
            _view.OnBackClick.RemoveListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            _soundService?.PlayEffectSound(SoundId.Click);
            // Handle back navigation
        }
    }
}