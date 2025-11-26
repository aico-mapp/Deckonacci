using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class iOSBuilder : MonoBehaviour
{
    [MenuItem("Build/iOS")]
    public static void BuildiOS()
    {
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersionString = "12.0";
        PlayerSettings.SplashScreen.show = false;
        
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = EditorBuildSettings.scenes.Select(s => s.path).ToArray(),
            locationPathName = "build/iOS",
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };

        
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
        }
        else
        {
            throw new System.Exception($"Build failed with {summary.result}");
        }
    }
}