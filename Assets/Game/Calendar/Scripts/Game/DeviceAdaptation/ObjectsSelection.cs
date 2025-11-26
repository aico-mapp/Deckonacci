using System;
using UnityEngine;

namespace Game.Calendar.Scripts.Game.DeviceAdaptation
{
    public class ObjectsSelection : DeviceSelection
    {
        [SerializeField] private GameObject _iphoneObjects;
        [SerializeField] private GameObject _ipadObjects;

        private void Awake()
        {
            if (IsIpad())
            {
                _ipadObjects.SetActive(true);
                _iphoneObjects.SetActive(false);
            }
            else
            {
                _ipadObjects.SetActive(false);
                _iphoneObjects.SetActive(true);
            }
        }
    }
}
