using System;
using UnityEngine;
using DeviceOrientation = UnityEngine.DeviceOrientation;

namespace Mode.Scripts.UI
{
    public class OrientationChanger : MonoBehaviour
    {
        public event Action<DeviceOrientation> OnOrientationChanged;
        
        private DeviceOrientation _currentOrientation;

        private void Awake() => _currentOrientation = Input.deviceOrientation;

        private void LateUpdate()
        {
            if (_currentOrientation == Input.deviceOrientation) return;
            
            OnOrientationChanged?.Invoke(Input.deviceOrientation);
            _currentOrientation = Input.deviceOrientation;
        }
    }
}