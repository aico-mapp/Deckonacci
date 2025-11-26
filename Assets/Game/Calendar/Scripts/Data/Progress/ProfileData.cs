using System;
using Calendar.Scripts.Data.Enums;
using Calendar.Scripts.Data.StaticData;
using UnityEngine;
using UnityEngine.Serialization;

namespace Calendar.Scripts.Data.Progress
{
    [Serializable]
    public class ProfileData
    {
        public string UserName;
        [FormerlySerializedAs("Flower")] 
        public int avatarID;
        public bool IsAvatarDefault;
        
        public ProfileData()
        {
            UserName = "";
            IsAvatarDefault = false;
            avatarID = -1;
        }
    }
}