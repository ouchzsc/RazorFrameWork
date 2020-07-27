// C# example.

using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;

public class BuildGame
{
    // Get filename.
    static string buildDir = "Build";
    static string gameName = "OuchzGame.exe";
    static string gamePath = System.IO.Path.Combine(buildDir, gameName);
    static string[] levels = new string[] {"Assets/Scenes/Boot.unity"};

    static string luaDir = "Lua";
    static string luaDest = System.IO.Path.Combine(buildDir, luaDir);

    [MenuItem("MyTools/Windows Build With Postprocess", false, 0)]
    public static void Build()
    {
        FileUtil.DeleteFileOrDirectory(luaDest);
        FileUtil.CopyFileOrDirectory(luaDir, luaDest);

        BuildReport report =
            BuildPipeline.BuildPlayer(levels, gamePath, BuildTarget.StandaloneWindows, BuildOptions.Development);

        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }

        // Run the game (Process class from System.Diagnostics).
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        proc.StartInfo.FileName = gamePath;
        proc.Start();
    }

    [PostProcessBuildAttribute(1)]
    public static void PostBuild_CopyLua(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log($"pathToBuiltProject:{pathToBuiltProject}");
        FileUtil.DeleteFileOrDirectory(luaDest);
        FileUtil.CopyFileOrDirectory(luaDir, luaDest);
    }
}