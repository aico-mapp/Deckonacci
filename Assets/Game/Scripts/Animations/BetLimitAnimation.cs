using TMPro;
using UnityEngine;
using DG.Tweening;

namespace Game.Scripts.Animations
{
    public class BetLimitAnimation : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _betLimitText;
        [SerializeField] private CanvasGroup _canvasGroup;
        
        [Header("Animation Settings")]
        [SerializeField] private float _showDuration = 0.5f;
        [SerializeField] private float _hideDuration = 0.4f;
        [SerializeField] private float _punchScale = 1.2f;
        [SerializeField] private float _shakeStrength = 20f;

        private Sequence _currentSequence;
        private Vector3 _originalScale;

        private void Awake()
        {
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
            
            _originalScale = Vector3.one;
            
            // Start hidden
            _canvasGroup.alpha = 0;
            transform.localScale = Vector3.zero;
        }

        public void Initialize(int betLimit)
        {
            _betLimitText.text = $"Maximum bet: {betLimit}";
        }

        public void Show()
        {
            // Kill any existing animation
            _currentSequence?.Kill();
            
            // Reset state
            transform.localScale = Vector3.zero;
            _canvasGroup.alpha = 0;
            
            // Create fancy animation sequence
            _currentSequence = DOTween.Sequence();
            
            // Phase 1: Pop in with elastic bounce
            _currentSequence.Append(transform.DOScale(_originalScale * _punchScale, _showDuration * 0.6f)
                .SetEase(Ease.OutBack));
            
            _currentSequence.Join(_canvasGroup.DOFade(1f, _showDuration * 0.4f)
                .SetEase(Ease.OutQuad));
            
            // Phase 2: Punch scale effect
            _currentSequence.Append(transform.DOPunchScale(Vector3.one * 0.15f, 0.3f, 5, 0.5f));
            
            // Phase 3: Shake for emphasis
            _currentSequence.Join(transform.DOShakePosition(0.3f, _shakeStrength, 10, 90, false, true));

            _currentSequence.JoinCallback(() =>
            {
                #if UNITY_IOS
                Handheld.Vibrate();
                #endif
            });
            
            // Phase 4: Color flash (optional - makes it more eye-catching)
            _currentSequence.Join(_betLimitText.DOColor(Color.red, 0.15f)
                .SetLoops(4, LoopType.Yoyo));
            
            // Phase 5: Settle to normal size
            _currentSequence.Append(transform.DOScale(_originalScale, 0.2f)
                .SetEase(Ease.InOutQuad));
            
            // Phase 7: Hide with smooth fade out and scale down
            _currentSequence.Append(transform.DOScale(_originalScale * 0.8f, _hideDuration)
                .SetEase(Ease.InBack));
            
            _currentSequence.Join(_canvasGroup.DOFade(0f, _hideDuration)
                .SetEase(Ease.InQuad));
            
            // Reset scale at the end
            _currentSequence.OnComplete(() => {
                transform.localScale = Vector3.zero;
            });
        }

        private void OnDestroy()
        {
            _currentSequence?.Kill();
        }
    }
}