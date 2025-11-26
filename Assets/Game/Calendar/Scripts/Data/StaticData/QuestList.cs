using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Calendar.Scripts.Data.StaticData
{
    [CreateAssetMenu(fileName = "QuestList", menuName = "StaticData/QuestList")]
    public class QuestList: ScriptableObject
    {
        public List<Quest> Quests;
    }

    [Serializable]
    public class Quest
    {
        public string Question;
        public int Answer;
    }
}