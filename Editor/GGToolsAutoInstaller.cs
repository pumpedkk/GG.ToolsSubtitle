#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;


namespace GGTools.Subtitle
{ 
[InitializeOnLoad]
public static class GGToolsAutoInstaller
{
    
    private static readonly (string name, string url)[] requiredPackages =
    {
        ("com.ggtools.docreader", "https://github.com/pumpedkk/GG.ToolsDocReader.git"),
        ("com.ggtools.spriteultilitis", "https://github.com/pumpedkk/GG.ToolsSpriteUltilitis.git"),
        ("com.ggtools.tmproultilitis", "https://github.com/pumpedkk/GG.ToolsTmproUltilitis.git")
    };

    private static ListRequest _listRequest;

    static GGToolsAutoInstaller()
    {
        EditorApplication.update += CheckPackages;
        _listRequest = Client.List();
    }

    private static void CheckPackages()
    {
        if (!_listRequest.IsCompleted)
            return;

        if (_listRequest.Status == StatusCode.Failure)
        {
            Debug.LogError("Package list error: " + _listRequest.Error.message);
            EditorApplication.update -= CheckPackages;
            return;
        }

        foreach (var (pkgName, gitURL) in requiredPackages)
        {
            bool found = false;

            foreach (var package in _listRequest.Result)
            {
                if (package.name == pkgName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.Log($"[GGTools Installer] Package '{pkgName}' not found. Installing from: {gitURL}");
                Client.Add(gitURL);
            }
        }

        EditorApplication.update -= CheckPackages;
    }
}
}
#endif
