using UnityEngine;

namespace Calendar.Scripts.Game.UI.Base
{
    public abstract class BaseView : MonoBehaviour
    {
        public virtual void Show() => gameObject.SetActive(true);
        
        public virtual void Hide() => gameObject.SetActive(false);
    }
}