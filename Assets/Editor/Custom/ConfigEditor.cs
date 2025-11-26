using System;
using System.Globalization;
using Mode.Scripts.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AppConfiguration))]
public class ConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
#if UNITY_IOS
        if (GUILayout.Button("Set iOS Project Settings")) SetIOSProjectSettings();
        if (GUILayout.Button("Check Project Settings")) CheckProjectSettings();
#endif
    }

    private void SetIOSProjectSettings()
    {
        PlayerSettings.SplashScreen.show = false;
        PlayerSettings.iOS.targetOSVersionString = "12.0";
        PlayerSettings.iOS.hideHomeButton = true;
        PlayerSettings.iOS.appInBackgroundBehavior = iOSAppInBackgroundBehavior.Custom;
        PlayerSettings.iOS.backgroundModes = iOSBackgroundMode.RemoteNotifications;
        Debug.Log("iOS Project Settings set successfully");
    }

    private void CheckProjectSettings()
    {
        AppConfiguration config = (AppConfiguration)target;
        DrawDefaultInspector();
        
        CheckFBKey(config);
        CheckStatusKey(config);
        CheckAppId(config);
    }

    private void CheckFBKey(AppConfiguration config)
    {
        string fbKey = config.firebaseKey;
        if(fbKey.Length == 0) Debug.LogError("Firebase key is empty");
        if (fbKey.Contains(" ")) Debug.LogError("Firebase key contains a space");
    }
    
    private void CheckStatusKey(AppConfiguration config)
    {
        string statusKey = config.statusKey;
        if(statusKey.Length == 0) Debug.LogError("Status key is empty");
        if (statusKey.Contains(" ")) Debug.LogError("Status key contains a space");
    }

    private void CheckAppId(AppConfiguration config)
    {
        string appId = config.appId;
        if(appId.Length == 0) Debug.LogError("App ID is empty");
        else if(appId.Contains(" ")) Debug.LogError("App Id contains a space");
    }
}
