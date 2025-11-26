using UnityEngine;

namespace Mode.Scripts.Data
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AppConfiguration", order = 7)]
    public class AppConfiguration : ScriptableObject
    {
        public int gameSceneIndex;
        public int mdlSceneIndex;
        [Space(12)]
        public ScreenOrientation gameScreenOrientation;
        public string firebaseKey;
        public string statusKey;
        public string appId;
    }
}