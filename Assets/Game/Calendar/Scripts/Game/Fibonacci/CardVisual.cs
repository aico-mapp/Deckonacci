using Calendar.Scripts.Data.Enums;
using DG.Tweening;
using Game.Calendar.Scripts.Services.Sound;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.Fibonacci
{
    public class CardVisual : MonoBehaviour
    {
        [SerializeField] private Image _cardImage;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _frontSide;
        [SerializeField] private GameObject _backSide;

        private FibonacciCard _card;
        private bool _isFaceUp;
        private System.Action<CardVisual> _onCardSelected;
        private RectTransform _rectTransform;

        public FibonacciCard Card => _card;
        public int Value => _card.Value;

        private ISoundService _soundService;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void Initialize(FibonacciCard card, System.Action<CardVisual> onSelected, ISoundService soundService)
        {
            _card = card;
            _onCardSelected = onSelected;
            _soundService = soundService;
            
            if (_valueText != null)
                _valueText.text = card.Value.ToString();
            
            if (_button != null)
            {
                _button.onClick.RemoveAllListeners();
                if (onSelected != null)
                    _button.onClick.AddListener(() => onSelected?.Invoke(this));
            }

            ShowBack();
        }

        public void ShowFront()
        {
            if (_isFaceUp) return;
            
            _isFaceUp = true;
            FlipCard(true);
        }

        public void ShowBack()
        {
            _isFaceUp = false;
            _frontSide?.SetActive(false);
            _backSide?.SetActive(true);
        }

        private void FlipCard(bool showFront)
        {
            float currentZ = _rectTransform.eulerAngles.z;
            
            _rectTransform.DORotate(new Vector3(0, 90, currentZ), 0.15f).OnComplete(() =>
            {
                _frontSide?.SetActive(showFront);
                _backSide?.SetActive(!showFront);
                _rectTransform.DORotate(new Vector3(0, 0, currentZ), 0.15f);
            });
        }

        public void SetInteractable(bool interactable)
        {
            if (_button != null)
                _button.interactable = interactable;
        }

        // Move using local position (for cards in container)
        public void AnimateToLocalPosition(Vector2 targetLocalPos, float targetRotationZ, float duration, System.Action onComplete = null)
        {
            if (_rectTransform == null) return;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_rectTransform.DOAnchorPos(targetLocalPos, duration).SetEase(Ease.OutQuad));
            sequence.Join(_rectTransform.DORotate(new Vector3(0, 0, targetRotationZ), duration).SetEase(Ease.OutQuad));
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        // Move using world position (for cards going to play field)
        public void MoveToWorldPosition(Vector3 targetWorldPos, float targetRotationZ, float duration, System.Action onComplete = null)
        {
            if (_rectTransform == null) return;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOMove(targetWorldPos, duration).SetEase(Ease.OutQuad));
            sequence.Join(_rectTransform.DORotate(new Vector3(0, 0, targetRotationZ), duration).SetEase(Ease.OutQuad));
            sequence.OnComplete(() => onComplete?.Invoke());
        }

        public void AnimateDeal(Vector2 targetLocalPos, float targetRotationZ, float delay, float duration)
        {
            if (_rectTransform == null) return;
            
            _rectTransform.localScale = Vector3.zero;
            
            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(delay);
            sequence.AppendCallback(() => _soundService.PlayEffectSound(SoundId.FibonacciCard));
            sequence.Append(_rectTransform.DOAnchorPos(targetLocalPos, duration).SetEase(Ease.OutBack));
            sequence.Join(_rectTransform.DOScale(Vector3.one, duration).SetEase(Ease.OutBack));
            sequence.Join(_rectTransform.DORotate(new Vector3(0, 0, targetRotationZ), duration).SetEase(Ease.OutBack));
        }

        public void HighlightWin()
        {
            _rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f);
            if (_cardImage != null)
                _cardImage.DOColor(Color.green, 0.3f).SetLoops(4, LoopType.Yoyo);
        }

        public void HighlightLose()
        {
            _rectTransform.DOShakeAnchorPos(0.3f, 10f, 10, 90, false, true);
            if (_cardImage != null)
                _cardImage.DOColor(Color.red, 0.3f).SetLoops(4, LoopType.Yoyo);
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
            DOTween.Kill(transform);
            DOTween.Kill(_rectTransform);
        }
    }
}