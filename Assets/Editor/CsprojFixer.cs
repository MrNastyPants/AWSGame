using UnityEditor;
using System.IO;

[InitializeOnLoad]
public class CsprojFixer {
    static CsprojFixer() {
        EditorApplication.projectChanged += OnProjectChanged;
    }

    static void OnProjectChanged() {
        FixCsprojFile();
    }

    static void FixCsprojFile() {
        string csprojPath = "Path/To/Your/Assembly-CSharp.csproj";
        if (File.Exists(csprojPath)) {
            string content = File.ReadAllText(csprojPath);
            content = content.Replace("&", "&amp;");  // Replace all occurrences of '&' with '&amp;'
            File.WriteAllText(csprojPath, content);
        }
    }
}