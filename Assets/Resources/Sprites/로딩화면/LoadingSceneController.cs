using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
    public string nextSceneName = "Work_Shop"; // Go_Next_Days에서 지정 가능
    public float minLoadingTime = 1.0f; // 최소 로딩 UI 표시 시간

    void Start()
    {
        Fader.FadeOnly(0.2f, 0f); // 알파1 → 0 (즉시 페이드인)
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        yield return null; // 1프레임 대기 (연출상 안정)

        var op = SceneManager.LoadSceneAsync(nextSceneName);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            timer += Time.deltaTime;

            // 로딩 진행률: op.progress는 0~0.9까지, 0.9가 준비 완료
            if (op.progress >= 0.9f && timer >= minLoadingTime)
            {
                // 페이드아웃 → 씬 활성화
                Fader.FadeOnly(0.25f, 0.25f, () => op.allowSceneActivation = true);
                yield break;
            }

            yield return null;
        }
    }
}
