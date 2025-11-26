using System.Collections.Generic;
using UnityEngine;

namespace Game.Calendar.Scripts.Data.StaticData
{
    [CreateAssetMenu(fileName = "GameGameSettings", menuName = "StaticData/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        public Players BotLevel;
        public Players FriendLevel;
    }
}