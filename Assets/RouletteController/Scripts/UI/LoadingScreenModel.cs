using System.Collections.Generic;
using Mode.Scripts.Data.Enums;
using UnityEngine;

namespace Mode.Scripts.UI
{
    public class LoadingScreenModel
    {
        public Rect ViewFrame = new(0, 0, Screen.width, Screen.height);
        public Dictionary<ScreenId, GameObject> Screens;
        public ScreenId ActiveScreen;
    }
}