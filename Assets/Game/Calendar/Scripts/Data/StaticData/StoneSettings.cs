using System;
using System.Collections.Generic;
using Game.Calendar.Scripts.Data.Enums;
using UnityEngine;

namespace Game.Calendar.Scripts.Data.StaticData
{
    [CreateAssetMenu(fileName = "StoneSettings", menuName = "StaticData/StoneSettings")]
    public class StoneSettings: ScriptableObject
    {
        public List<StoneData> BotStones;
    }

    [Serializable]
    public class StoneData
    {
        public StoneType Type;
        public Sprite BotSprite;
        public Sprite FriendSprite;
        public Sprite[] Fragments;
    }
}   