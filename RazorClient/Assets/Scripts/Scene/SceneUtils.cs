using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneUtils
{
    public static void loadScene(String sceneName, Action done)
    {
        Boot.inst.StartCoroutine(LoadSceneCoroutine(sceneName, done));
    }

    private static IEnumerator LoadSceneCoroutine(string sceneName, Action done)
    {
        var oper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (oper.isDone == false)
        {
            yield return null;
        }

        done?.Invoke();
    }

    public static void unloadScene(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }

    public static void listenSceneUnload(Action<string> action)
    {
        SceneManager.sceneUnloaded += scene => action?.Invoke(scene.name);
    }

    public static void listenSceneLoaded(Action<string> action)
    {
        SceneManager.sceneLoaded += (scene, loadMode) => action?.Invoke(scene.name);
    }
}