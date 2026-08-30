using UnityEditor;
using System.IO;

public class BuildScript
{
    static string[] GetScenes()
    {
        var scenes = EditorBuildSettings.scenes;
        var paths = new string[scenes.Length];
        for (int i = 0; i < scenes.Length; i++) paths[i] = scenes[i].path;
        return paths;
    }
    
    [MenuItem("Build/Windows")]
    static void BuildWindows()
    {
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = BuildTarget.StandaloneWindows64;
        options.locationPathName = "Builds/Windows/CardClash.exe";
        options.scenes = GetScenes();
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/Mac")]
    static void BuildMac()
    {
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = BuildTarget.StandaloneOSX;
        options.locationPathName = "Builds/Mac/CardClash.app";
        options.scenes = GetScenes();
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/WebGL")]
    static void BuildWebGL()
    {
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = BuildTarget.WebGL;
        options.locationPathName = "Builds/WebGL";
        options.scenes = GetScenes();
        BuildPipeline.BuildPlayer(options);
    }

    [MenuItem("Build/Android")]
    static void BuildAndroid()
    {
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = BuildTarget.Android;
        options.locationPathName = "Builds/Android/CardClash.apk";
        options.scenes = GetScenes();
        BuildPipeline.BuildPlayer(options);
    }
}