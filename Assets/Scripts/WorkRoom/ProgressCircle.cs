using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressCircle : MonoBehaviour
{
    public Image fillImage;
    public Image Image;
    public Image completeImage;

    public float maxProgress = 30f;
 

    private bool isRunning = false;
    public System.Action OnComplete;  // 외부에서 할당 가능

    public void CompleteCircle()
    {
        if (!isRunning)
            StartCoroutine(FillOverTime());
        else
        {
            Debug.Log("현재 실행 중입니다. 작업을 완료해주세요.");
        }
    }

    public void ProgressInit()
    {
        isRunning = false;

        fillImage.fillAmount = 1;
        Image.gameObject.SetActive(true);
        fillImage.gameObject.SetActive(true);
        completeImage.gameObject.SetActive(false);
    }

    IEnumerator FillOverTime()
    {
        isRunning = true;
        float elapsed = 0f;

        while (elapsed < maxProgress)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(1 - (elapsed / maxProgress));
            fillImage.fillAmount = progress;

            // 항상 카메라를 향하게
            transform.forward = Camera.main.transform.forward;

            yield return null;
        }

        // 작업 완료 시 처리
        fillImage.gameObject.SetActive(false);
        Image.gameObject.SetActive(false);
        completeImage.gameObject.SetActive(true);


        if (OnComplete != null)
        {
            OnComplete.Invoke();
        }

    }


}