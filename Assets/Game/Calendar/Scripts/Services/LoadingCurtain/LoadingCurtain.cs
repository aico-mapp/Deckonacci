using System.Collections;
using Calendar.Scripts.Game.UI.Base;
using UnityEngine;

namespace Calendar.Scripts.Services.LoadingCurtain
{
    public class LoadingCurtain : BaseView, ILoadingCurtain
    {
        [SerializeField] private CanvasGroup _curtain;

        public override void Show()
        {
            gameObject.SetActive(true);
            _curtain.alpha = 1;
        }
        
        public override void Hide()
        {
            if(gameObject.activeSelf) StartCoroutine(DoFadeIn());
        }

        private IEnumerator DoFadeIn()
        {
            while (_curtain.alpha > 0)
            {
                _curtain.alpha -= Time.deltaTime * 3;
                yield return null;
            }
      
            gameObject.SetActive(false);
        }
    }
}