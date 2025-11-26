using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.Scripts.Animations
{
    public class PopupAnimation : MonoBehaviour
    {
        [Header("Animation Settings")]
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private float scaleMultiplier = 1.1f;
        [SerializeField] private Ease showEase = Ease.OutBack;
        [SerializeField] private Ease hideEase = Ease.InBack;
        
        [Header("Components")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private List<RectTransform> popupRects = new List<RectTransform>();
        [SerializeField] private bool addSelfAsPopupRect = true;
        [SerializeField] private float elementDelay = 0.1f; // Delay between animating multiple elements
        
        private Dictionary<RectTransform, Vector3> _originalScales = new Dictionary<RectTransform, Vector3>();
        private Sequence _currentAnimation;
        
        private void Awake()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();
            
            // If no popup rects specified and addSelfAsPopupRect is true, add self
            if (popupRects.Count == 0 && addSelfAsPopupRect)
            {
                RectTransform selfRect = GetComponent<RectTransform>();
                if (selfRect != null)
                {
                    popupRects.Add(selfRect);
                }
            }
            
            // Store original scale for each popup rect
            foreach (var rect in popupRects)
            {
                if (rect != null)
                {
                    _originalScales[rect] = rect.localScale;
                }
            }
            
            // Initialize hidden state
            canvasGroup.alpha = 0f;
            foreach (var rect in popupRects)
            {
                if (rect != null)
                {
                    rect.localScale = Vector3.zero;
                }
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
        
        public void Show()
        {
            gameObject.SetActive(true);
            
            KillCurrentAnimation();
            
            canvasGroup.alpha = 0f;
            foreach (var rect in popupRects)
            {
                if (rect != null)
                {
                    rect.localScale = Vector3.zero;
                }
            }
            
            _currentAnimation = DOTween.Sequence();
    
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
            
            // Fade in the canvas group
            _currentAnimation.Append(canvasGroup.DOFade(1f, animationDuration * 0.5f));
            
            // Animate each popup rect with a slight delay between them
            for (int i = 0; i < popupRects.Count; i++)
            {
                var rect = popupRects[i];
                if (rect == null) continue;
                
                Vector3 originalScale = _originalScales[rect];
                float delay = i * elementDelay;
                
                _currentAnimation.Insert(delay, rect.DOScale(originalScale * scaleMultiplier, animationDuration * 0.7f)
                        .SetEase(showEase))
                    .Insert(delay + animationDuration * 0.7f, rect.DOScale(originalScale, animationDuration * 0.3f));
            }
            
            _currentAnimation.OnStart(() =>
            {
                gameObject.SetActive(true);
                canvasGroup.blocksRaycasts = true;
                canvasGroup.interactable = true;
            });
        }
        
        public void Hide()
        {
            KillCurrentAnimation();
            
            _currentAnimation = DOTween.Sequence();
            
            // Fade out the canvas group
            _currentAnimation.Append(canvasGroup.DOFade(0f, animationDuration * 0.7f));
            
            // Animate each popup rect with a slight delay between them
            for (int i = 0; i < popupRects.Count; i++)
            {
                var rect = popupRects[i];
                if (rect == null) continue;
                
                float delay = i * elementDelay * 0.5f; // Use a shorter delay for hiding
                
                _currentAnimation.Insert(delay, rect.DOScale(Vector3.zero, animationDuration)
                    .SetEase(hideEase));
            }
            
            _currentAnimation.OnStart(() => {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            })
            .OnComplete(() => {
                gameObject.SetActive(false);
            });
        }

        public void ImmediateHide()
        {
            gameObject.SetActive(false);
        }
        
        public void CancelAnimation()
        {
            KillCurrentAnimation();
        }
        
        private void KillCurrentAnimation()
        {
            if (_currentAnimation != null && _currentAnimation.IsActive())
            {
                _currentAnimation.Kill();
                _currentAnimation = null;
            }
        }
        
        private void OnDestroy()
        {
            KillCurrentAnimation();
        }
        
        public void AddPopupRect(RectTransform rect)
        {
            if (rect != null && !popupRects.Contains(rect))
            {
                popupRects.Add(rect);
                _originalScales[rect] = rect.localScale;
            }
        }
        
        public void RemovePopupRect(RectTransform rect)
        {
            if (rect != null && popupRects.Contains(rect))
            {
                popupRects.Remove(rect);
                _originalScales.Remove(rect);
            }
        }
    }
}