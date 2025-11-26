using System.IO;
using System.Xml;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class AndroidManifestPermissions : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
#if UNITY_ANDROID
        RunPostProcessTasksAndroid();
#endif
    }

    private static void RunPostProcessTasksAndroid()
    {
        var appManifestPath = Path.Combine(Application.dataPath, "Plugins/Android/AndroidManifest.xml");

        var manifestFile = new XmlDocument();
        manifestFile.Load(appManifestPath);

        var manifestHasChanged = false;

        manifestHasChanged |= AddPermissions(manifestFile);
        if (manifestHasChanged) manifestFile.Save(appManifestPath);
    }

    private static bool AddPermissions(XmlDocument manifest)
    {
        var manifestHasChanged = false;

        manifestHasChanged |= AddPermission(manifest, "android.permission.ACCESS_NETWORK_STATE");
        manifestHasChanged |= AddPermission(manifest, "android.permission.INTERNET");
        manifestHasChanged |= AddPermission(manifest, "android.permission.CAMERA");
        manifestHasChanged |= AddPermission(manifest, "android.permission.WRITE_EXTERNAL_STORAGE");
        manifestHasChanged |= AddPermission(manifest, "android.permission.READ_EXTERNAL_STORAGE");
        manifestHasChanged |= AddPermission(manifest, "android.permission.ACCESS_WIFI_STATE");
        manifestHasChanged |= AddPermission(manifest, "android.permission.MICROPHONE");
        manifestHasChanged |= AddPermission(manifest, "android.permission.RECORD_AUDIO");
        manifestHasChanged |= AddPermission(manifest, "android.permission.MODIFY_AUDIO_SETTINGS");
        manifestHasChanged |= AddPermission(manifest, "com.google.android.gms.permission.AD_ID");

        return manifestHasChanged;
    }

    private static bool AddPermission(XmlDocument manifest, string permissionValue)
    {
        if (DoesPermissionExist(manifest, permissionValue))
        {
            Debug.Log(string.Format("[Adjust]: Your app's AndroidManifest.xml file already contains {0} permission.",
                permissionValue));
            return false;
        }

        XmlElement element = manifest.CreateElement("uses-permission");
        AddAndroidNamespaceAttribute(manifest, "name", permissionValue, element);
        manifest.DocumentElement.AppendChild(element);
        Debug.Log(string.Format("[Adjust]: {0} permission successfully added to your app's AndroidManifest.xml file.",
            permissionValue));

        return true;
    }

    private static bool DoesPermissionExist(XmlDocument manifest, string permissionValue)
    {
        var xpath = string.Format("/manifest/uses-permission[@android:name='{0}']", permissionValue);
        return manifest.DocumentElement.SelectSingleNode(xpath, GetNamespaceManager(manifest)) != null;
    }

    private static void AddAndroidNamespaceAttribute(XmlDocument manifest, string key, string value, XmlElement node)
    {
        XmlAttribute androidSchemeAttribute =
            manifest.CreateAttribute("android", key, "http://schemas.android.com/apk/res/android");
        androidSchemeAttribute.InnerText = value;
        node.SetAttributeNode(androidSchemeAttribute);
    }

    private static XmlNamespaceManager GetNamespaceManager(XmlDocument manifest)
    {
        var namespaceManager = new XmlNamespaceManager(manifest.NameTable);
        namespaceManager.AddNamespace("android", "http://schemas.android.com/apk/res/android");
        return namespaceManager;
    }
}