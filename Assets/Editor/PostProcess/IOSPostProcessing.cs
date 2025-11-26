using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

public static class IOSPostProcessing
{
    private const string XcodeProjectPath = "/Unity-iPhone.xcodeproj/project.pbxproj";
    private const string XcodeTargetName = "Unity-iPhone";

    private const string CameraKey = "NSCameraUsageDescription";
    private const string CameraValue = "$(PRODUCT_NAME) camera use";

    private const string PhotoKey = "NSPhotoLibraryUsageDescription";
    private const string PhotoValue = "$(PRODUCT_NAME) photo use";

    private const string MicroKey = "NSMicrophoneUsageDescription";
    private const string MicroValue = "$(PRODUCT_NAME) microphone use";

    [PostProcessBuild]
    public static void AddAssociatedDomains(BuildTarget buildTarget, string pathToBuildProject)
    {
        if (buildTarget == BuildTarget.iOS) AddPushNotification(pathToBuildProject);
    }

    private static void AddPushNotification(string pathToBuiltProject)
    {
        ProjectCapabilityManager entitlements = new ProjectCapabilityManager(pathToBuiltProject + XcodeProjectPath, Application.identifier,
            XcodeTargetName);
        entitlements.AddPushNotifications(true);
        entitlements.AddBackgroundModes(BackgroundModesOptions.BackgroundFetch);
        entitlements.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
        entitlements.WriteToFile();
    }

    [PostProcessBuild]
    private static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuildProject)
    {
        if (buildTarget != BuildTarget.iOS) return;
        string plistPath = pathToBuildProject + "/Info.plist";
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        PlistElementDict rootDict = plist.root;

        rootDict.SetString(CameraKey, CameraValue);
        rootDict.SetString(PhotoKey, PhotoValue);
        rootDict.SetString(MicroKey, MicroValue);

        File.WriteAllText(plistPath, plist.WriteToString());
    }

    [PostProcessBuild(45)]
    private static void AddUseFrameworksToPodfile(BuildTarget target, string buildPath)
    {
        if (target != BuildTarget.iOS) return;
        using StreamWriter sw = File.AppendText(buildPath + "/Podfile");
        sw.WriteLine("use_frameworks!");
    }
}