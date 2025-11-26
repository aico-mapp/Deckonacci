using UnityEngine;

namespace Game.Calendar.Scripts.Game.DeviceAdaptation
{
    public class DeviceSelection : MonoBehaviour
    {
        protected bool _isPortraitOrientation = true;
        protected const float IPadAspectRatioMin = 1.3f;
        protected const float IPadAspectRatioMax = 1.5f;
        
        public bool IsIpad()
        {
            float aspectRatio = GetAspectRatio();
            bool meetsAspectRatio = aspectRatio >= IPadAspectRatioMin && aspectRatio <= IPadAspectRatioMax;
            
            return meetsAspectRatio;
        }
        
        protected float GetAspectRatio()
        {
            float longer = Mathf.Max(Screen.width, Screen.height);
            float shorter = Mathf.Min(Screen.width, Screen.height);
            return longer / shorter;
        }
    }
}