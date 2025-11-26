using DG.Tweening;
using UnityEngine;

namespace Game.Scripts.Animations
{
    public class LogoAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float flyInDuration = 1f;
        [SerializeField] private float centerPauseDuration = 2f;
        [SerializeField] private float flyOutDuration = 1f;
        [SerializeField] private float loopDelay = 0.5f; // Delay before loop restarts
    
        [Header("Movement Settings")]
        [SerializeField] private float flyInDistance = 15f; // Distance from right
        [SerializeField] private float flyOutDistance = 15f; // Distance to left
        private Vector3 centerPosition = Vector3.zero; // Logo center position
    
        [Header("Easing")]
        [SerializeField] private Ease flyInEase = Ease.OutQuad;
        [SerializeField] private Ease flyOutEase = Ease.InQuad;
    
        [Header("Additional Effects")]
        [SerializeField] private bool enableScaleEffect = true;
        [SerializeField] private float scaleMultiplier = 1.2f;
        [SerializeField] private bool enableRotation = false;
        [SerializeField] private float rotationAmount = 360f;
        [SerializeField] private bool enableFade = true;
    
        [Header("Auto Start")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private float startDelay = 0f;
    
        private Sequence logoSequence;
        private Vector3 originalScale;
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
    
        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                Debug.LogError("LogoAnimation requires a RectTransform component!");
                return;
            }
        
            // Store original scale
            originalScale = transform.localScale;
        
            // Setup CanvasGroup for fading
            if (enableFade)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }
        }
    
        void Start()
        {
            centerPosition = rectTransform.anchoredPosition;
            if (playOnStart)
            {
                if (startDelay > 0)
                {
                    DOVirtual.DelayedCall(startDelay, () => PlayAnimation());
                }
                else
                {
                    PlayAnimation();
                }
            }
        }
    
        public void PlayAnimation()
        {
            // Kill any existing sequence
            logoSequence?.Kill();
        
            // Reset to start position (off-screen right)
            Vector3 startPos = centerPosition + Vector3.right * flyInDistance;
            Vector3 endPos = centerPosition + Vector3.left * flyOutDistance;
        
            rectTransform.anchoredPosition = startPos;
        
            if (enableFade)
            {
                canvasGroup.alpha = 0f;
            }
        
            if (enableScaleEffect)
            {
                transform.localScale = originalScale * 0.5f;
            }
        
            // Create animation sequence
            logoSequence = DOTween.Sequence();
        
            // === FLY IN FROM RIGHT ===
            logoSequence.Append(
                rectTransform.DOAnchorPos(centerPosition, flyInDuration)
                    .SetEase(flyInEase)
            );
        
            if (enableFade)
            {
                logoSequence.Join(
                    canvasGroup.DOFade(1f, flyInDuration * 0.7f)
                        .SetEase(Ease.OutQuad)
                );
            }
        
            if (enableScaleEffect)
            {
                logoSequence.Join(
                    transform.DOScale(originalScale * scaleMultiplier, flyInDuration * 0.5f)
                        .SetEase(Ease.OutBack)
                );
                logoSequence.Append(
                    transform.DOScale(originalScale, flyInDuration * 0.3f)
                        .SetEase(Ease.InOutQuad)
                );
            }
        
            if (enableRotation)
            {
                logoSequence.Join(
                    transform.DORotate(new Vector3(0, 0, rotationAmount), flyInDuration)
                        .SetEase(flyInEase)
                );
            }
        
            // === PAUSE IN CENTER ===
            logoSequence.AppendInterval(centerPauseDuration);
        
            // === FLY OUT TO LEFT ===
            logoSequence.Append(
                rectTransform.DOAnchorPos(endPos, flyOutDuration)
                    .SetEase(flyOutEase)
            );
        
            if (enableFade)
            {
                logoSequence.Join(
                    canvasGroup.DOFade(0f, flyOutDuration * 0.8f)
                        .SetEase(Ease.InQuad)
                        .SetDelay(flyOutDuration * 0.2f)
                );
            }
        
            if (enableScaleEffect)
            {
                logoSequence.Join(
                    transform.DOScale(originalScale * 0.5f, flyOutDuration)
                        .SetEase(Ease.InQuad)
                );
            }
        
            if (enableRotation)
            {
                logoSequence.Join(
                    transform.DORotate(new Vector3(0, 0, rotationAmount * 2), flyOutDuration)
                        .SetEase(flyOutEase)
                );
            }
        
            // === LOOP DELAY ===
            logoSequence.AppendInterval(loopDelay);
        
            // === LOOP ===
            logoSequence.SetLoops(-1, LoopType.Restart);
            logoSequence.OnStepComplete(() =>
            {
                if (enableRotation)
                {
                    // Reset rotation for next loop
                    transform.rotation = Quaternion.identity;
                }
            });
        }
    
        public void StopAnimation()
        {
            logoSequence?.Kill();
        }
    
        public void PauseAnimation()
        {
            logoSequence?.Pause();
        }
    
        public void ResumeAnimation()
        {
            logoSequence?.Play();
        }
    
        public void RestartAnimation()
        {
            StopAnimation();
            PlayAnimation();
        }
    
        void OnDestroy()
        {
            logoSequence?.Kill();
        }
    
        void OnDisable()
        {
            logoSequence?.Pause();
        }
    
        void OnEnable()
        {
            if (logoSequence != null && logoSequence.IsActive())
            {
                logoSequence.Play();
            }
        }
    }
}