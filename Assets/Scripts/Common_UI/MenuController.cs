using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using UnityEditor.Tilemaps;
using System.Linq;


public class MenuController : MonoBehaviour
{
    public GameObject menuButton; // MenuItems 오브젝트
    public List<GameObject> menuButtons; // 메뉴 버튼들
    public GameObject menuAlertIcon; // 알림 아이콘
    public GameObject interiorButton;
    public float spacing = 60f;          // 버튼 사이 간격
    public float delay = 0.05f;          // 애니메이션 간 딜레이
    public float fadeTime = 0.5f;        // 투명도 애니메이션 시간
    private bool isMenuOpen = false;

    public GameObject storeSign;
    public Sprite openSprite;     // 클릭 후 보여줄 이미지
    public Image targetImage;       // 현재 버튼의 이미지
    public Sprite closeSprite;   // 원래 이미지 저장
    private bool isOpen;
    private bool isRotating = false;


    public GameObject mapPanel;

    public GameObject energy;
    public GameObject energyLevel;
    public TMP_Text energyLevelText;
    public TMP_Text energyPercentText;

    public TMP_Text GoldText;
    public TMP_Text MoonrockText;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.getInstance();

        isOpen = gameManager.Get_IsOpen();
        targetImage.sprite = isOpen ? openSprite : closeSprite;

        gameManager.OnshopCloseHours += ChangeImage;
    }

    // Update is called once per frame
    void Update()
    {
        GoldText.text = gameManager.Get_Gold().ToString();
        MoonrockText.text = gameManager.Get_Moonrock().ToString();

        int hours = gameManager.Get_Hours();
        int minutes = gameManager.Get_Minutes();

        if (hours == 8 && minutes == 0)
            storeSign.SetActive(true);

        string currentSceneName = SceneManager.GetActiveScene().name;  // 현재 씬
        if (currentSceneName == "Work_Shop" || currentSceneName == "Work_Room")
        {
            if (!menuButtons.Contains(interiorButton))
            {
                interiorButton.SetActive(true);
                RectTransform baseRT = menuButton.GetComponent<RectTransform>();
                Vector2 basePos = baseRT.anchoredPosition;
                interiorButton.GetComponent<RectTransform>().anchoredPosition = basePos + new Vector2(0, -spacing * (menuButtons.Count + 1));
                interiorButton.GetComponent<CanvasGroup>().alpha = 0;
                menuButtons.Add(interiorButton);
            }
        }
        else
        {
            if (menuButtons.Contains(interiorButton))
            {
                menuButtons.Remove(interiorButton);
            }
        }

        if (currentSceneName == "Moonlight_Hill" || currentSceneName == "Cloud_Pond")
        {
            energy.SetActive(false);
            energyLevel.SetActive(false);
        }
        else
        {
            if (energyLevel.activeSelf)
            {
                energy.SetActive(false);
            }
            else
            {
                energy.SetActive(true);
            }
        }

        UpdateNotification();
    }

    // 알림 아이콘 업데이트
    public void UpdateNotification()
    {
        bool shouldShow = false;

        foreach (var menu in menuButtons)
        {

            var alerts = menu.GetComponentsInChildren<Transform>(true)
                         .Where(t => t.name == "Alert");

            if (alerts.Any(alert => alert.gameObject.activeSelf))
            {
                shouldShow = true;
                break;
            }
        }

        menuAlertIcon.SetActive(shouldShow);
        Debug.Log($"🔔 menuAlertIcon.SetActive({shouldShow})");
    }


    public void ToggleMenuItems()
    {
        StopAllCoroutines();

        if (isMenuOpen)
            StartCoroutine(HideMenu());
        else
            StartCoroutine(ShowMenu());

        isMenuOpen = !isMenuOpen;
    }

    IEnumerator ShowMenu()
    {
        for (int i = 0; i < menuButtons.Count; i++)
        {
            RectTransform baseRT = menuButton.GetComponent<RectTransform>();
            Vector2 basePos = baseRT.anchoredPosition;

            GameObject btn = menuButtons[i];
            RectTransform rt = btn.GetComponent<RectTransform>();
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();

            btn.SetActive(true);
            rt.anchoredPosition = basePos;
            cg.alpha = 0;

            // 위치 이동
            Vector2 targetPos = basePos + new Vector2(0, -spacing * (i + 1));
            StartCoroutine(SlideTo(rt, targetPos, fadeTime));
            // 투명도 애니메이션
            StartCoroutine(FadeTo(cg, 1f, fadeTime));

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator HideMenu()
    {
        for (int i = menuButtons.Count - 1; i >= 0; i--)
        {
            GameObject btn = menuButtons[i];
            RectTransform rt = btn.GetComponent<RectTransform>();
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();

            StartCoroutine(SlideTo(rt, menuButton.GetComponent<RectTransform>().anchoredPosition, fadeTime));
            StartCoroutine(FadeTo(cg, 0f, fadeTime));

            yield return new WaitForSeconds(delay);
            btn.SetActive(false);
        }
    }

    IEnumerator FadeTo(CanvasGroup cg, float targetAlpha, float duration)
    {
        float start = cg.alpha;
        float time = 0f;
        while (time < duration)
        {
            cg.alpha = Mathf.Lerp(start, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cg.alpha = targetAlpha;
    }

    IEnumerator SlideTo(RectTransform rt, Vector2 targetPos, float duration)
    {
        Vector2 start = rt.anchoredPosition;
        float time = 0f;
        while (time < duration)
        {
            rt.anchoredPosition = Vector2.Lerp(start, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        rt.anchoredPosition = targetPos;
    }

    public GameObject storeSignPopup;

    public void ShowStoreSignPopup()
    {
        // 오픈 가능 시간 아니라면 클릭되지 않음
        if (gameManager.Get_Hours() < 9 || gameManager.Get_Hours() >= 18) return;

        if (storeSignPopup != null)
        {
            storeSignPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("Store Sign Popup is not assigned in the inspector.");
        }

        TextMeshProUGUI confirmText = storeSignPopup.GetComponentInChildren<TextMeshProUGUI>();
        if (!isOpen)
        {
            if (confirmText != null)
            {
                confirmText.text = $"가게를 여시겠습니까?";

                Button yesButton = storeSignPopup.transform.Find("YesButton").GetComponent<Button>();
                yesButton.onClick.AddListener(() => { ChangeImage(); storeSignPopup.SetActive(false); gameManager.Set_IsOpen(true); });

                // No 버튼 클릭 시 팝업 닫기
                Button noButton = storeSignPopup.transform.Find("NoButton").GetComponent<Button>();
                noButton.onClick.AddListener(() => { storeSignPopup.SetActive(false); });

            }
            else
            {
                Debug.LogError("Text 컴포넌트가 storeSignPopup 안에 없습니다.");
            }
        }
        else
        {
            if (confirmText != null)
            {
                confirmText.text = $"가게를 닫으시겠습니까?";

                Button yesButton = storeSignPopup.transform.Find("YesButton").GetComponent<Button>();
                yesButton.onClick.AddListener(() => { ChangeImage(); storeSignPopup.SetActive(false); gameManager.Set_IsOpen(false); });

                // No 버튼 클릭 시 팝업 닫기
                Button noButton = storeSignPopup.transform.Find("NoButton").GetComponent<Button>();
                noButton.onClick.AddListener(() => { storeSignPopup.SetActive(false); });
            }
            else
            {
                Debug.LogError("Text 컴포넌트가 storeSignPopup 안에 없습니다.");
            }
        }
    }

    public void ChangeImage()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateAndSwap());
        }
    }

    IEnumerator RotateAndSwap()
    {
        isRotating = true;

        float duration = 0.5f;
        float time = 0f;

        RectTransform rt = targetImage.GetComponent<RectTransform>();
        float startY = 0f;
        float endY = 90f;

        // 1단계: 0 → 90도 회전
        while (time < duration)
        {
            float angle = Mathf.Lerp(startY, endY, time / duration);
            rt.localEulerAngles = new Vector3(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.localEulerAngles = new Vector3(0, 90f, 0);

        // 이미지 교체
        isOpen = gameManager.Get_IsOpen();
        targetImage.sprite = isOpen ? openSprite : closeSprite;

        // 2단계: 90 → 180도 회전
        time = 0f;
        startY = 270f;
        endY = 360f;

        while (time < duration)
        {
            float angle = Mathf.Lerp(startY, endY, time / duration);
            rt.localEulerAngles = new Vector3(0, angle, 0);
            time += Time.deltaTime;
            yield return null;
        }

        rt.localEulerAngles = new Vector3(0, 0, 0); // 180도 대신 0으로 복원
        isRotating = false;
    }


    public void MapPanelClose()
    {
        if (mapPanel != null)
            mapPanel.SetActive(false);
    }

    public void MapPanelOpen()
    {
        if (mapPanel != null)
            mapPanel.SetActive(true);

    }



    public void SeeEnergyLevel()
    {
        if (energyLevel.activeSelf)
        {
            energyLevel.SetActive(false);
            energy.SetActive(true);
        }
        else
        {
            energyLevel.SetActive(true);
            energy.SetActive(false);

            energyLevelText.text = "Lv " + gameManager.Get_EnergyLevel().ToString();
            energyPercentText.text = gameManager.Get_EnergyPercent().ToString() + "%";
        }
    }

    public GameObject confirmPopup;

    public void OnPlaceClicked(GameObject clickedObject)
    {
        string placeName = clickedObject.name;
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (placeName != currentSceneName)
        {
            ShowConfirmPopup(placeName);
        }
        else
        {
            Debug.Log("현재 씬과 같은 장소이므로 팝업을 띄우지 않습니다.");
        }
    }

    private void ShowConfirmPopup(string placeName)
    {
        confirmPopup.SetActive(true);

        TextMeshProUGUI confirmText = confirmPopup.GetComponentInChildren<TextMeshProUGUI>();
        if (confirmText != null)
        {
            switch (placeName)
            {
                case "Moonlight_Hill":
                    confirmText.text = $"달빛언덕에 가시겠습니까?";
                    break;
                case "Sleeping_Garden":
                    confirmText.text = $"수면정원에 가시겠습니까?";
                    break;
                case "Cloud_Pond":
                    confirmText.text = $"구름연못에 가시겠습니까?";
                    break;
                case "Village":
                    confirmText.text = $"마을에 가시겠습니까?";
                    break;
                case "Work_Shop":
                    confirmText.text = $"이불가게에 가시겠습니까?";
                    break;
                default:
                    Debug.LogError("이동할 장소 씬이 없습니다.");
                    Debug.LogError($"Unknown place name: {placeName}");
                    break;
            }

        }
        else
        {
            Debug.LogError("Text 컴포넌트가 confirmPopup 안에 없습니다.");
        }


        // Yes 버튼 클릭 시 이동
        Button yesButton = confirmPopup.transform.Find("YesButton").GetComponent<Button>();
        yesButton.onClick.AddListener(() => { OnConfirm(placeName); });

        // No 버튼 클릭 시 팝업 닫기
        Button noButton = confirmPopup.transform.Find("NoButton").GetComponent<Button>();
        noButton.onClick.AddListener(() => { confirmPopup.SetActive(false); });


    }

    private void OnConfirm(string placeName)
    {
        gameManager.Set_EndScene(placeName);
        //SceneManager.LoadScene(placeName);
        Fader.GoConcurrent(placeName);
    }
}
