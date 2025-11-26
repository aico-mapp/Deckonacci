using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#if UNITY_IOS
#endif

public static class DisableBitcode
{
    [PostProcessBuild(999)]
    public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject)
    {
#if UNITY_IOS
        if (buildTarget != BuildTarget.iOS) return;
        var projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
        var pbxProject = new PBXProject();
        pbxProject.ReadFromFile(projectPath);

        //Disabling Bitcode on all targets
        //Main
        var target = pbxProject.GetUnityMainTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        //Unity Tests
        target = pbxProject.TargetGuidByName(PBXProject.GetUnityTestTargetName());
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
        //Unity Framework
        target = pbxProject.GetUnityFrameworkTargetGuid();
        pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
        pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

        pbxProject.WriteToFile(projectPath);
#endif
    }
}