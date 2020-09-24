using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneUtils
{
    public static void loadScene(String sceneName, Action done)
    {
        Boot.inst.StartCoroutine(SwitchSceneCoroutine(sceneName, done));
    }

    private static IEnumerator SwitchSceneCoroutine(string sceneName, Action done)
    {
        var oper = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (oper.isDone==false)
        {
            yield return null;
        }
        done();
    }
}