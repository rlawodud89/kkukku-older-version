// ProgressCircle.cs
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ProgressCircle : MonoBehaviour
{
    public Image fillImage;
    public Image Image;
    public Image completeImage;

    private float maxProgress = 30f;
    private GameManager gameManager;
    private bool isRunning = false;
    public Action OnComplete;

    private int lastLevel = -1;

    private void Start()
    {
        if (gameManager == null)
            gameManager = GameManager.getInstance();

        // 시작 시 레벨 체크
        UpdateMaxProgress();
        ProgressInit();
    }

    // UpgradeShopController에서 호출할 public 메서드
    public void RefreshMaxProgress()
    {
        UpdateMaxProgress();
    }

    public void UpdateMaxProgress()
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

    public void CompleteCircle()
    {
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
        float elapsed = 0f;

        while (elapsed < maxProgress)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(1 - (elapsed / maxProgress));
            fillImage.fillAmount = progress;

            transform.forward = Camera.main.transform.forward;
            yield return null;
        }

        fillImage.gameObject.SetActive(false);
        Image.gameObject.SetActive(false);
        completeImage.gameObject.SetActive(true);

        OnComplete?.Invoke();
    }
}
