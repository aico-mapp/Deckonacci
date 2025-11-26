using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace EditorBuildOnCICD
{
    public static class PostBuildActions
    {
        [PostProcessBuild]
        public static void PostProcess(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.iOS) return;
            byte[] data = Encoding.UTF8.GetBytes(Application.version);
            string path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "SupportFiles", "version.txt");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            File.WriteAllBytes(path, data);
        }
    }

}