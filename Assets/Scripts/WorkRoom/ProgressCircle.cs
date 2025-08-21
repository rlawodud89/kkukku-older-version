using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressCircle : MonoBehaviour
{
    public Image fillImage;
    public Image Image;
    public Image completeImage;

    public float maxProgress;

    private bool isRunning = false;
    public System.Action OnComplete;  // 외부에서 할당 가능

    private GameManager gameManager;
    private int workerID;


    private float elapsed = 0f;
    private int lastLevel = -1;

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.getInstance();

        // 시작 시 레벨 체크
        UpdateMaxProgress();
        ProgressInit();
    }

    public void RefreshMaxProgress()
    {
        UpdateMaxProgress();
    }

    private void UpdateMaxProgress()
    {
        int level = 1;
        switch (gameObject.tag)
        {
            case "Fox": level = gameManager.Get_LoomLevel(); break;
            case "Sheep": level = gameManager.Get_FillerLevel(); break;
            case "Cat": level = gameManager.Get_DecoLevel(); break;
        }

        if (level == lastLevel) return; // 변경 없으면 무시
        lastLevel = level;

        switch (level)
        {
            case 1: maxProgress = 30f; break;
            case 2: maxProgress = 25f; break;
            case 3: maxProgress = 20f; break;
            case 4: maxProgress = 15f; break;
            case 5: maxProgress = 10f; break;
            default: maxProgress = 30f; break;
        }

        Debug.Log($"[{gameObject.tag}] level={level}, maxProgress={maxProgress}");
    }
    public void CompleteCircle(int workerID, float startElapsed = 0f)
    {
        this.workerID = workerID;
        elapsed = startElapsed;

        if (!isRunning)
            StartCoroutine(FillOverTime());
        else
            Debug.Log("현재 실행 중입니다. 작업을 완료해주세요.");
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
        Debug.LogWarning("Start:" + elapsed);

        while (elapsed < maxProgress)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(1 - (elapsed / maxProgress));
            fillImage.fillAmount = progress;
            gameManager.Set_Worker_WorkingPercent(workerID, elapsed);

            // 항상 카메라를 향하게
            transform.forward = Camera.main.transform.forward;

            Debug.LogWarning(elapsed);

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