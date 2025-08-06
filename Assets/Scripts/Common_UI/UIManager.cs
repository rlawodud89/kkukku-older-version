using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private static bool isUILoaded = false;

    void Awake()
    {
        if (!isUILoaded)
        {
            isUILoaded = true;
            StartCoroutine(LoadCommonUI());
        }
    }

    IEnumerator LoadCommonUI()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Common_UI", LoadSceneMode.Additive);
        yield return new WaitUntil(() => asyncLoad.isDone);
    }
}

