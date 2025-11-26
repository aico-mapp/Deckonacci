using DG.Tweening;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.UI.Base
{
    public abstract class FadeBaseView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeSpeed;
        private Tween _fadeTween;
        
        private void Awake() => OnAwake();

        protected virtual void OnAwake()
        {
            _canvasGroup.alpha = 0;
            gameObject.SetActive(false);
        }
        
        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (_fadeTween != null) return;
            _fadeTween = _canvasGroup.DOFade(1, _fadeSpeed).OnComplete(StopFadeTween);
        }

        public virtual void Hide()
        {
            if (_fadeTween != null) return;
            _fadeTween = _canvasGroup.DOFade(0, _fadeSpeed)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    StopFadeTween();
                });
        }

        private void StopFadeTween()
        {
            _fadeTween.Kill();
            _fadeTween = null;
        }
    }
}