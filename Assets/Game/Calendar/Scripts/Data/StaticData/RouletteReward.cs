using System;
using Game.Calendar.Scripts.Game.Table;
using UnityEngine;

namespace Game.Calendar.Scripts.Data.StaticData
{
    [Serializable]
    public class RouletteReward
    {
        public int Number;
        public Sprite RewardIcon;
        public string RewardDescription;
        public int RewardAmount = -1;
        public CategoryColor RewardColor;
        public CategoryType RewardType;
        public SuitType RewardSuit;

        [Space(5f)]
        public int Odds;
    }
}