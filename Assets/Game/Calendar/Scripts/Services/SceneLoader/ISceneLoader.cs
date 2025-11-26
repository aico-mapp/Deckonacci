using System;
using Game.Calendar.Scripts.Services;

namespace Calendar.Scripts.Services.SceneLoader
{
    public interface ISceneLoader : IGlobalService
    {
        void LoadScene(string sceneName, Action onLoaded = null);
    }
}