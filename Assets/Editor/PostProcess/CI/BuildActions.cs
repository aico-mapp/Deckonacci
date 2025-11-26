using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace EditorBuildOnCICD
{
    public class BuildActions
    {
        public static string APP_FOLDER = Directory.GetCurrentDirectory();
        public static string IOS_FOLDER = string.Format("{0}/Builds/iOS/", APP_FOLDER);

        public static string[] GetScenes()
        {
            List<string> scenes = new List<string>();
            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                {
                    scenes.Add(EditorBuildSettings.scenes[i].path);
                }
            }
            return scenes.ToArray();
        }
        
        public static void iOSRelease()
        {
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.iOS, ScriptingImplementation.IL2CPP);
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.iOS, string.Empty);
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            var report = BuildPipeline.BuildPlayer(GetScenes(), IOS_FOLDER, BuildTarget.iOS, BuildOptions.None);
            var code = (report.summary.result == BuildResult.Succeeded) ? 0 : 1;
            EditorApplication.Exit(code);
        }
    }
}