using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Calendar.Scripts.Game.UI.Elements
{
    public class ToggleView : MonoBehaviour
    {
        public Toggle Toggle;
        
        private int _id;
        
        public void Configure(int id)
        {
            _id = id;
        }
    
        public void Select()
        {
            Toggle.SetIsOnWithoutNotify(true);
        }

        public void Unselect()
        {
            Toggle.SetIsOnWithoutNotify(false);
        }
        
    }
}