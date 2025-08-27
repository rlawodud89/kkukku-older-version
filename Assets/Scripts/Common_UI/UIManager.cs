using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private static bool isUILoaded = false;

    void Awake()
    {
        /*
        if (!isUILoaded)
        {
            isUILoaded = true;
            StartCoroutine(LoadCommonUI());
        }*/

        StartCoroutine(LoadCommonUI());
    }

    IEnumerator LoadCommonUI()
    {
        var uiScene = SceneManager.GetSceneByName("Common_UI");

        if (!uiScene.IsValid() || !uiScene.isLoaded)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Common_UI", LoadSceneMode.Additive);
            yield return new WaitUntil(() => asyncLoad.isDone);
        }
    }


}

