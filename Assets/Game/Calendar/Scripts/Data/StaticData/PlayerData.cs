using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Calendar.Scripts.Data.StaticData
{
    [Serializable]
    public class Players
    {
        public List<PlayerData> playersData;
    }
    
    [Serializable]
    public class PlayerData
    {
        public int PlayerID;
        public string PlayerName;
        public Sprite PlayerSprite;
    }
}