using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Calendar.Scripts.Game.DeviceAdaptation
{
    public class CanvasScalerResolution: DeviceSelection
    {
        private CanvasScaler _canvasScaler;
        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();

            _canvasScaler.matchWidthOrHeight = IsIpad() ? 0.25f : 0.5f;
        }
    }
}