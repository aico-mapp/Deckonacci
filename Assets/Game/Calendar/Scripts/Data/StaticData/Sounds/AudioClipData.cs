using System;
using Calendar.Scripts.Data.Enums;
using UnityEngine;

namespace Calendar.Scripts.Data.StaticData.Sounds
{
    [Serializable]
    public class AudioClipData
    {
        public AudioClip Clip;
        public SoundId Id;
    }
}